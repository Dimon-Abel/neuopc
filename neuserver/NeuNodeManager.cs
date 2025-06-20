using Opc.Ua;
using Opc.Ua.Server;
using neulib;

namespace neuserver
{
    internal class NeuNodeManager : CustomNodeManager2
    {
        private IList<IReference>? _references;
        private FolderState? _folder;
        private readonly List<BaseDataVariableState> _variables;
        private readonly ValueWrite? _write;
        private readonly ReaderWriterLockSlim _rwLock = new();

        public NeuNodeManager(
            IServerInternal server,
            ApplicationConfiguration configuration,
            ValueWrite write
        )
            : base(
                server,
                configuration,
                "http://opcfoundation.org/Quickstarts/ReferenceApplications"
            )
        {
            _write = write;
            _variables = new List<BaseDataVariableState>();
        }

        public override void Browse(OperationContext context, ref ContinuationPoint continuationPoint, IList<ReferenceDescription> references)
        {
            base.Browse(context, ref continuationPoint, references);
        }

        public override ServiceResult SubscribeToAllEvents(OperationContext context, uint subscriptionId, IEventMonitoredItem monitoredItem, bool unsubscribe)
        {
            return base.SubscribeToAllEvents(context, subscriptionId, monitoredItem, unsubscribe);
        }

        private void AddNode(Item item)
        {
            var variable = new BaseDataVariableState(_folder)
            {
                SymbolicName = item.Name,
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                NodeId = new NodeId(item.Name, NamespaceIndex),
                BrowseName = new QualifiedName(item.Name, NamespaceIndex),
                DisplayName = new LocalizedText("en", item.Name),
                WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
                UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
                DataType = DataTypeIds.Union,
                ValueRank = ValueRanks.Scalar,
                AccessLevel = AccessLevels.CurrentReadOrWrite,
                UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                Historizing = false,
                StatusCode = StatusCodes.Good,
                Timestamp = DateTime.Now,
                OnWriteValue = OnWriteDataValue,
                OnReadValue = OnReadDataValue
            };

            SetVariable(variable, item);

            if (variable.ValueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (variable.ValueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }
            else { }

            _folder?.AddChild(variable);

            _variables.Add(variable);
        }

        public void SaveNode(Item item, BaseDataVariableState? variable)
        {
            if (variable == null)
            {
                _rwLock.EnterWriteLock();
                try
                {
                    AddNode(item);
                    AddPredefinedNode(SystemContext, _folder);
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }
            else
            {
                // 只读操作无需加锁，或用 EnterReadLock() 保护
                SetVariable(variable, item);
                variable.ClearChangeMasks(SystemContext, false);
            }
        }

        public void UpdateNodes(List<Item> list)
        {
            // 在并行处理前创建 _variables 的快照
            List<BaseDataVariableState> variablesSnapshot;
            lock (Lock)
            {
                variablesSnapshot = new List<BaseDataVariableState>(_variables);
            }

            Parallel.ForEach(list, item =>
            {
                // 只查找快照，不会抛出集合被修改的异常
                var variable = variablesSnapshot.FirstOrDefault(v => v.SymbolicName.Equals(item.Name));
                SaveNode(item, variable);
            });
        }

        public override void CreateAddressSpace(
            IDictionary<NodeId, IList<IReference>> externalReferences
        )
        {
            lock (Lock)
            {
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out _references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = _references =
                        new List<IReference>();
                }

                string folderName = "NeuOPC";
                _folder = new FolderState(null)
                {
                    SymbolicName = folderName,
                    ReferenceTypeId = ReferenceTypes.Organizes,
                    TypeDefinitionId = ObjectTypeIds.FolderType,
                    NodeId = new NodeId(folderName, NamespaceIndex),
                    BrowseName = new QualifiedName(folderName, NamespaceIndex),
                    DisplayName = new LocalizedText("en", folderName),
                    WriteMask = AttributeWriteMask.None,
                    UserWriteMask = AttributeWriteMask.None,
                    EventNotifier = EventNotifiers.None
                };

                _folder.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                _references.Add(
                    new NodeStateReference(ReferenceTypes.Organizes, false, _folder.NodeId)
                );
                _folder.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(_folder);
                AddPredefinedNode(SystemContext, _folder);
            }
        }

        private static void SetVariable(BaseDataVariableState variable, Item item)
        {
            variable.StatusCode = item.Quality switch
            {
                Quality.GoodLocalOverrideValueForced => StatusCodes.GoodLocalOverride,
                Quality.Good => StatusCodes.Good,
                Quality.UncertainValueFromMultipleSources
                    => StatusCodes.UncertainConfigurationError,
                Quality.UncertainEngineeringUnitsExceeded
                    => StatusCodes.UncertainEngineeringUnitsExceeded,
                Quality.UncertainSensorNotAccurate => StatusCodes.UncertainSensorNotAccurate,
                Quality.UncertainLastUsableValue => StatusCodes.UncertainLastUsableValue,
                Quality.Uncertain => StatusCodes.Uncertain,
                Quality.BadOutOfService => StatusCodes.BadOutOfService,
                Quality.BadCommFailure => StatusCodes.BadCommunicationError,
                Quality.BadLastKnowValuePassed => StatusCodes.BadDependentValueChanged,
                Quality.BadSensorFailure => StatusCodes.BadSensorFailure,
                Quality.BadDeviceFailure => StatusCodes.BadDeviceFailure,
                Quality.BadNotConnected => StatusCodes.BadNotConnected,
                Quality.Bad => StatusCodes.Bad,
                Quality.BadConfigurationErrorInServer => StatusCodes.BadConfigurationError,
                _ => StatusCodes.Good,
            };

            variable.Timestamp = item.Timestamp;

            string type = "";
            if (null != item.Type)
            {
                type = item.Type.ToString();
            }

            switch (type)
            {
                case "System.SByte":
                    variable.DataType = DataTypeIds.SByte;
                    variable.Value = (sbyte)(item.Value ?? 0);
                    break;
                case "System.Int16":
                    variable.DataType = DataTypeIds.Int16;
                    variable.Value = (short)(item.Value ?? 0);
                    break;
                case "System.Int32":
                    variable.DataType = DataTypeIds.Int32;
                    variable.Value = (int)(item.Value ?? 0);
                    break;
                case "System.Int64":
                    variable.DataType = DataTypeIds.Int64;
                    variable.Value = (long)(item.Value ?? 0);
                    break;
                case "System.Single":
                    variable.DataType = DataTypeIds.Float;
                    variable.Value = (float)(item.Value ?? 0);
                    break;
                case "System.Double":
                    variable.DataType = DataTypeIds.Double;
                    variable.Value = (double)(item.Value ?? 0);
                    break;
                case "System.Byte":
                    variable.DataType = DataTypeIds.Byte;
                    variable.Value = (byte)(item.Value ?? 0);
                    break;
                case "System.UInt16":
                    variable.DataType = DataTypeIds.UInt16;
                    variable.Value = (ushort)(item.Value ?? 0);
                    break;
                case "System.UInt32":
                    variable.DataType = DataTypeIds.UInt32;
                    variable.Value = (uint)(item.Value ?? 0);
                    break;
                case "System.UInt64":
                    variable.DataType = DataTypeIds.UInt64;
                    variable.Value = (ulong)(item.Value ?? 0);
                    break;
                case "System.DateTime":
                {
                    variable.DataType = DataTypeIds.DateTime;
                    try
                    {
                        variable.Value = (DateTime)(item.Value ?? DateTime.Now);
                    }
                    catch
                    {
                        variable.StatusCode = StatusCodes.BadNotReadable;
                    }

                    break;
                }
                case "System.String":
                    variable.DataType = DataTypeIds.String;
                    variable.Value = item.Value as string;
                    break;
                case "System.Boolean":
                {
                    variable.DataType = DataTypeIds.Boolean;
                    try
                    {
                        variable.Value = (bool)(item.Value ?? false);
                    }
                    catch
                    {
                        variable.StatusCode = StatusCodes.BadNotReadable;
                    }

                    break;
                }
                case "":
                    variable.DataType = DataTypeIds.Union;
                    variable.Value = item.Value as Union;
                    break;

                default:
                {
                    variable.DataType = DataTypeIds.BaseDataType;
                    variable.StatusCode = StatusCodes.BadNotSupported;
                    break;
                }
            }
        }

        private ServiceResult OnReadDataValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp
        )
        {
            return ServiceResult.Good;
        }

        private ServiceResult OnWriteDataValue(
            ISystemContext context,
            NodeState node,
            NumericRange indexRange,
            QualifiedName dataEncoding,
            ref object value,
            ref StatusCode statusCode,
            ref DateTime timestamp
        )
        {
            if (node is not BaseDataVariableState variable)
            {
                return StatusCodes.BadNodeIdInvalid;
            }

            var item = new Item();
            try
            {
                TypeInfo typeInfo = TypeInfo.IsInstanceOfDataType(
                    value,
                    variable.DataType,
                    variable.ValueRank,
                    context.NamespaceUris,
                    context.TypeTable
                );

                if (typeInfo == null || typeInfo == TypeInfo.Unknown)
                {
                    return StatusCodes.BadTypeMismatch;
                }

                item.Name = variable.SymbolicName;
                item.Value = value;

                switch (typeInfo.BuiltInType)
                {
                    case BuiltInType.SByte:
                        item.Type = typeof(sbyte);
                        break;
                    case BuiltInType.Int16:
                        item.Type = typeof(short);
                        break;
                    case BuiltInType.Int32:
                        item.Type = typeof(int);
                        break;
                    case BuiltInType.Int64:
                        item.Type = typeof(long);
                        break;
                    case BuiltInType.Float:
                        item.Type = typeof(float);
                        break;
                    case BuiltInType.Double:
                        item.Type = typeof(double);
                        break;
                    case BuiltInType.Byte:
                        item.Type = typeof(byte);
                        break;
                    case BuiltInType.UInt16:
                        item.Type = typeof(ushort);
                        break;
                    case BuiltInType.UInt32:
                        item.Type = typeof(uint);
                        break;
                    case BuiltInType.UInt64:
                        item.Type = typeof(ulong);
                        break;
                    case BuiltInType.DateTime:
                        item.Type = typeof(DateTime);
                        break;
                    case BuiltInType.String:
                        item.Type = typeof(string);
                        break;
                    case BuiltInType.Boolean:
                        item.Type = typeof(bool);
                        break;
                    default:
                        return StatusCodes.BadTypeMismatch;
                }

                var result = _write?.Invoke(item);
                if (result is true)
                {
                    return ServiceResult.Good;
                }

                return StatusCodes.BadNotWritable;
            }
            catch (Exception)
            {
                return StatusCodes.BadTypeMismatch;
            }
        }
    }
}
