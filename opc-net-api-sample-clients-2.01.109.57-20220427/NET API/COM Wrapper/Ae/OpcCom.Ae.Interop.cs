//============================================================================
// TITLE: OpcCom.Ae.Interop.cs
//
// CONTENTS:
// 
// A class that implements various COM marshalling/unmarshalling functions.
//
// (c) Copyright 2004 The OPC Foundation
// ALL RIGHTS RESERVED.
//
// DISCLAIMER:
//  This code is provided by the OPC Foundation solely to assist in 
//  understanding and use of the appropriate OPC Specification(s) and may be 
//  used as set forth in the License Grant section of the OPC Specification.
//  This code is provided as-is and without warranty or support of any sort
//  and is subject to the Warranty and Liability Disclaimers which appear
//  in the printed OPC Specification.
//
// MODIFICATION LOG:
//
// Date       By    Notes
// ---------- ---   -----
// 2004/11/08 RSA   Initial implementation.

#pragma warning disable 0618

using System;
using System.Threading;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Reflection;
using Opc;
using Opc.Ae;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace OpcCom.Ae
{
	/// <summary>
	/// Defines COM marshalling/unmarshalling functions for AE.
	/// </summary>
	public class Interop
	{		
		/// <summary>
		/// Converts a standard FILETIME to an OpcRcw.Ae.FILETIME structure.
		/// </summary>
		internal static OpcRcw.Ae.FILETIME Convert(FILETIME input)
		{
			OpcRcw.Ae.FILETIME output = new OpcRcw.Ae.FILETIME();
			output.dwLowDateTime   = input.dwLowDateTime;
			output.dwHighDateTime  = input.dwHighDateTime;
			return output;
		}

		/// <summary>
		/// Converts an OpcRcw.Ae.FILETIME to a standard FILETIME structure.
		/// </summary>
		internal static FILETIME Convert(OpcRcw.Ae.FILETIME input)
		{
			FILETIME output       = new FILETIME();
			output.dwLowDateTime  = input.dwLowDateTime;
			output.dwHighDateTime = input.dwHighDateTime;
			return output;
		}
		
		/// <summary>
		/// Converts the HRESULT to a system type.
		/// </summary>
		internal static ResultID GetResultID(int input)
		{
			// must check for this error because of a code collision with a DA code.
			if (input == ResultIDs.E_INVALIDBRANCHNAME)
			{
				return ResultID.Ae.E_INVALIDBRANCHNAME;
			}
			
			return OpcCom.Interop.GetResultID(input);
		}

		/// <summary>
		/// Unmarshals and deallocates a OPCEVENTSERVERSTATUS structure.
		/// </summary>
		internal static ServerStatus GetServerStatus(ref IntPtr pInput, bool deallocate)
		{
			ServerStatus output = null;
			
			if (pInput != IntPtr.Zero)
			{
				OpcRcw.Ae.OPCEVENTSERVERSTATUS status = (OpcRcw.Ae.OPCEVENTSERVERSTATUS)Marshal.PtrToStructure(pInput, typeof(OpcRcw.Ae.OPCEVENTSERVERSTATUS));
		
				output = new ServerStatus();

				output.VendorInfo     = status.szVendorInfo;
				output.ProductVersion = String.Format("{0}.{1}.{2}", status.wMajorVersion, status.wMinorVersion, status.wBuildNumber);
				output.ServerState    = (ServerState)status.dwServerState;
				output.StatusInfo     = null;
				output.StartTime      = OpcCom.Interop.GetFILETIME(Convert(status.ftStartTime));
				output.CurrentTime    = OpcCom.Interop.GetFILETIME(Convert(status.ftCurrentTime));
				output.LastUpdateTime = OpcCom.Interop.GetFILETIME(Convert(status.ftLastUpdateTime));

				if (deallocate)
				{
					Marshal.DestroyStructure(pInput, typeof(OpcRcw.Ae.OPCEVENTSERVERSTATUS));
					Marshal.FreeCoTaskMem(pInput);
					pInput = IntPtr.Zero;
				}
			}

			return output;
		}

		/// <summary>
		/// Converts a NodeType value to the OPCAEBROWSETYPE equivalent.
		/// </summary>
		internal static OpcRcw.Ae.OPCAEBROWSETYPE GetBrowseType(BrowseType input)
		{			
			switch (input)
			{
				case BrowseType.Area:   return OpcRcw.Ae.OPCAEBROWSETYPE.OPC_AREA;
				case BrowseType.Source: return OpcRcw.Ae.OPCAEBROWSETYPE.OPC_SOURCE;
			}

			return OpcRcw.Ae.OPCAEBROWSETYPE.OPC_AREA;
		}

		/// <summary>
		/// Converts an array of ONEVENTSTRUCT structs to an array of EventNotification objects.
		/// </summary>
		internal static EventNotification[] GetEventNotifications(OpcRcw.Ae.ONEVENTSTRUCT[] input)
		{
			EventNotification[] output = null;
			
			if (input != null && input.Length > 0)
			{
				output = new EventNotification[input.Length];

				for (int ii = 0; ii < input.Length; ii++)
				{
					output[ii] = GetEventNotification(input[ii]);
				}
			}

			return output;
		}

		/// <summary>
		/// Converts a ONEVENTSTRUCT struct to a EventNotification object.
		/// </summary>
		internal static EventNotification GetEventNotification(OpcRcw.Ae.ONEVENTSTRUCT input)
		{
			EventNotification output = new EventNotification();

			output.SourceID         = input.szSource;
			output.Time             = OpcCom.Interop.GetFILETIME(Convert(input.ftTime));
			output.Severity         = input.dwSeverity;
			output.Message          = input.szMessage;					
			output.EventType        = (EventType)input.dwEventType;
			output.EventCategory    = input.dwEventCategory;
			output.ChangeMask       = input.wChangeMask;
			output.NewState         = input.wNewState;
			output.Quality          = new Opc.Da.Quality(input.wQuality);
			output.ConditionName    = input.szConditionName;
			output.SubConditionName = input.szSubconditionName;
			output.AckRequired      = input.bAckRequired != 0;
			output.ActiveTime       = OpcCom.Interop.GetFILETIME(Convert(input.ftActiveTime));
			output.Cookie           = input.dwCookie;
			output.ActorID          = input.szActorID;

			object[] attributes = OpcCom.Interop.GetVARIANTs(ref input.pEventAttributes, input.dwNumEventAttrs, false);

			output.SetAttributes(attributes);

			return output;
		}

		/// <summary>
		/// Converts an array of OpcRcw.Ae.OPCCONDITIONSTATE structs to an array of Opc.Ae.Condition objects.
		/// </summary>
		internal static Condition[] GetConditions(ref IntPtr pInput, int count, bool deallocate)
		{
			Condition[] output = null;
			
			if (pInput != IntPtr.Zero && count > 0)
			{
				output = new Condition[count];

				IntPtr pos = pInput;

				for (int ii = 0; ii < count; ii++)
				{
					OpcRcw.Ae.OPCCONDITIONSTATE condition = (OpcRcw.Ae.OPCCONDITIONSTATE)Marshal.PtrToStructure(pos, typeof(OpcRcw.Ae.OPCCONDITIONSTATE));

					output[ii] = new Condition();

					output[ii].State             = condition.wState;
					output[ii].Quality           = new Opc.Da.Quality(condition.wQuality);
					output[ii].Comment           = condition.szComment;
					output[ii].AcknowledgerID    = condition.szAcknowledgerID;
					output[ii].CondLastActive    = OpcCom.Interop.GetFILETIME(Convert(condition.ftCondLastActive));
					output[ii].CondLastInactive  = OpcCom.Interop.GetFILETIME(Convert(condition.ftCondLastInactive));
					output[ii].SubCondLastActive = OpcCom.Interop.GetFILETIME(Convert(condition.ftSubCondLastActive));
					output[ii].LastAckTime       = OpcCom.Interop.GetFILETIME(Convert(condition.ftLastAckTime));

					output[ii].ActiveSubCondition.Name        = condition.szActiveSubCondition;
					output[ii].ActiveSubCondition.Definition  = condition.szASCDefinition;
					output[ii].ActiveSubCondition.Severity    = condition.dwASCSeverity;
					output[ii].ActiveSubCondition.Description = condition.szASCDescription;

					// unmarshal sub-conditions.
					string[] names        = OpcCom.Interop.GetUnicodeStrings(ref condition.pszSCNames, condition.dwNumSCs, deallocate);
					int[]    severities   = OpcCom.Interop.GetInt32s(ref condition.pdwSCSeverities, condition.dwNumSCs, deallocate); 
					string[] definitions  = OpcCom.Interop.GetUnicodeStrings(ref condition.pszSCDefinitions, condition.dwNumSCs, deallocate);
					string[] descriptions = OpcCom.Interop.GetUnicodeStrings(ref condition.pszSCDescriptions, condition.dwNumSCs, deallocate);

					output[ii].SubConditions.Clear();

					if (condition.dwNumSCs > 0)
					{
						for (int jj = 0; jj < names.Length; jj++)
						{
							SubCondition subcondition = new SubCondition();

							subcondition.Name        = names[jj];
							subcondition.Severity    = severities[jj];
							subcondition.Definition  = definitions[jj];
							subcondition.Description = descriptions[jj];

							output[ii].SubConditions.Add(subcondition);
						}
					}
					
					// unmarshal attributes.
					object[] values = OpcCom.Interop.GetVARIANTs(ref condition.pEventAttributes, condition.dwNumEventAttrs, deallocate);
					int[]    errors = OpcCom.Interop.GetInt32s(ref condition.pErrors, condition.dwNumEventAttrs, deallocate);

					output[ii].Attributes.Clear();

					if (condition.dwNumEventAttrs > 0)
					{
						for (int jj = 0; jj < values.Length; jj++)
						{
							AttributeValue attribute = new AttributeValue();

							attribute.ID       = 0;
							attribute.Value    = values[jj];
							attribute.ResultID = OpcCom.Ae.Interop.GetResultID(errors[jj]);

							output[ii].Attributes.Add(attribute);
						}
					}
					
					// deallocate structure.
					if (deallocate)
					{
						Marshal.DestroyStructure(pos, typeof(OpcRcw.Ae.OPCCONDITIONSTATE));
					}

					pos = (IntPtr)(pos.ToInt64() + Marshal.SizeOf(typeof(OpcRcw.Ae.OPCCONDITIONSTATE)));
				}

				// deallocate array.
				if (deallocate)
				{
					Marshal.FreeCoTaskMem(pInput);
					pInput = IntPtr.Zero;
				}
			}

			return output;
		}

        /*
        /// <summary>
        /// Converts an array of COM HRESULTs structures to .NET ResultID objects.
        /// </summary>
        internal static ResultID[] GetResultIDs(ref IntPtr pInput, int count, bool deallocate)
        {
            ResultID[] output = null;
			
            if (pInput != IntPtr.Zero && count > 0)
            {
                output = new ResultID[count];

                int[] errors = OpcCom.Interop.GetInt32s(ref pInput, count, deallocate);

                for (int ii = 0; ii < count; ii++)
                {
                    output[ii] = OpcCom.Interop.GetResultID(errors[ii]);
                }
            }

            return output;
        }		
		
        /// <summary>
        /// Converts an array of COM SourceServer structures to .NET SourceServer objects.
        /// </summary>
        internal static SourceServer[] GetSourceServers(ref IntPtr pInput, int count, bool deallocate)
        {
            SourceServer[] output = null;
			
            if (pInput != IntPtr.Zero && count > 0)
            {
                output = new SourceServer[count];

                IntPtr pos = pInput;

                for (int ii = 0; ii < count; ii++)
                {
                    OpcRcw.Dx.SourceServer server = (OpcRcw.Dx.SourceServer)Marshal.PtrToStructure(pos, typeof(OpcRcw.Dx.SourceServer));

                    output[ii] = new SourceServer();

                    output[ii].ItemName         = server.szItemName;
                    output[ii].ItemPath         = server.szItemPath;
                    output[ii].Version          = server.szVersion;
                    output[ii].Name             = server.szName;
                    output[ii].Description      = server.szDescription;
                    output[ii].ServerType       = server.szServerType;
                    output[ii].ServerURL        = server.szServerURL;
                    output[ii].DefaultConnected = server.bDefaultSourceServerConnected != 0;

                    pos = (IntPtr)(pos.ToInt64() + Marshal.SizeOf(typeof(OpcRcw.Dx.SourceServer)));
                }

                if (deallocate)
                {
                    Marshal.FreeCoTaskMem(pInput);
                    pInput = IntPtr.Zero;
                }
            }

            return output;
        }		

        /// <summary>
        /// Converts an array of .NET SourceServer objects to COM SourceServer structures.
        /// </summary>
        internal static OpcRcw.Dx.SourceServer[] GetSourceServers(SourceServer[] input)
        {
            OpcRcw.Dx.SourceServer[] output = null;
			
            if (input != null && input.Length > 0)
            {
                output = new OpcRcw.Dx.SourceServer[input.Length];

                for (int ii = 0; ii < input.Length; ii++)
                {
                    output[ii] =  new OpcRcw.Dx.SourceServer();

                    output[ii].dwMask                        = (uint)OpcRcw.Dx.Mask.All;
                    output[ii].szItemName                    = input[ii].ItemName;
                    output[ii].szItemPath                    = input[ii].ItemPath;
                    output[ii].szVersion                     = input[ii].Version;
                    output[ii].szName                        = input[ii].Name;
                    output[ii].szDescription                 = input[ii].Description;
                    output[ii].szServerType                  = input[ii].ServerType;
                    output[ii].szServerURL                   = input[ii].ServerURL;
                    output[ii].bDefaultSourceServerConnected = (input[ii].DefaultConnected)?1:0;					
                }
            }

            return output;
        }		

        /// <summary>
        /// Converts an array of COM DXGeneralResponse structure to a .NET GeneralResponse object.
        /// </summary>
        internal static GeneralResponse GetGeneralResponse(OpcRcw.Dx.DXGeneralResponse input, bool deallocate)
        {		
            Opc.Dx.IdentifiedResult[] results = Interop.GetIdentifiedResults(ref input.pIdentifiedResults, input.dwCount, deallocate);

            return  new GeneralResponse(input.szConfigurationVersion, results);
        }	

        /// <summary>
        /// Converts an array of COM IdentifiedResult structures to .NET IdentifiedResult objects.
        /// </summary>
        internal static Opc.Dx.IdentifiedResult[] GetIdentifiedResults(ref IntPtr pInput, int count, bool deallocate)
        {
            Opc.Dx.IdentifiedResult[] output = null;
			
            if (pInput != IntPtr.Zero && count > 0)
            {
                output = new Opc.Dx.IdentifiedResult[count];

                IntPtr pos = pInput;

                for (int ii = 0; ii < count; ii++)
                {
                    OpcRcw.Dx.IdentifiedResult result = (OpcRcw.Dx.IdentifiedResult)Marshal.PtrToStructure(pos, typeof(OpcRcw.Dx.IdentifiedResult));

                    output[ii] = new Opc.Dx.IdentifiedResult();
 
                    output[ii].ItemName = result.szItemName;
                    output[ii].ItemPath = result.szItemPath;
                    output[ii].Version  = result.szVersion;
                    output[ii].ResultID = OpcCom.Interop.GetResultID(result.hResultCode);
					
                    if (deallocate)
                    {
                        Marshal.DestroyStructure(pos, typeof(OpcRcw.Dx.IdentifiedResult));
                    }

                    pos = (IntPtr)(pos.ToInt64() + Marshal.SizeOf(typeof(OpcRcw.Dx.IdentifiedResult)));
                }

                if (deallocate)
                {
                    Marshal.FreeCoTaskMem(pInput);
                    pInput = IntPtr.Zero;
                }
            }

            return output;
        }		

        /// <summary>
        /// Converts an array of COM DXConnection structures to .NET DXConnection objects.
        /// </summary>
        internal static DXConnection[] GetDXConnections(ref IntPtr pInput, int count, bool deallocate)
        {
            DXConnection[] output = null;
			
            if (pInput != IntPtr.Zero && count > 0)
            {
                output = new DXConnection[count];

                IntPtr pos = pInput;

                for (int ii = 0; ii < count; ii++)
                {
                    OpcRcw.Dx.DXConnection connection = (OpcRcw.Dx.DXConnection)Marshal.PtrToStructure(pos, typeof(OpcRcw.Dx.DXConnection));

                    output[ii] = GetDXConnection(connection, deallocate);
					
                    if (deallocate)
                    {
                        Marshal.DestroyStructure(pos, typeof(OpcRcw.Dx.DXConnection));
                    }

                    pos = (IntPtr)(pos.ToInt64() + Marshal.SizeOf(typeof(OpcRcw.Dx.DXConnection)));
                }

                if (deallocate)
                {
                    Marshal.FreeCoTaskMem(pInput);
                    pInput = IntPtr.Zero;
                }
            }

            return output;
        }		

        /// <summary>
        /// Converts an array of .NET DXConnection objects to COM DXConnection structures.
        /// </summary>
        internal static OpcRcw.Dx.DXConnection[] GetDXConnections(DXConnection[] input)
        {
            OpcRcw.Dx.DXConnection[] output = null;
			
            if (input != null && input.Length > 0)
            {
                output = new OpcRcw.Dx.DXConnection[input.Length];

                for (int ii = 0; ii < input.Length; ii++)
                {
                    output[ii] = GetDXConnection(input[ii]);
                }
            }

            return output;
        }	
	
        /// <summary>
        /// Converts a .NET DXConnection object to COM DXConnection structure.
        /// </summary>
        internal static OpcRcw.Dx.DXConnection GetDXConnection(DXConnection input)
        {			
            OpcRcw.Dx.DXConnection output = new OpcRcw.Dx.DXConnection();

            // set output default values.
            output.dwMask = 0;
            output.szItemPath = null;
            output.szItemName = null;
            output.szVersion = null;
            output.dwBrowsePathCount = 0;
            output.pszBrowsePaths = IntPtr.Zero;
            output.szName = null;
            output.szDescription = null;
            output.szKeyword = null;
            output.bDefaultSourceItemConnected = 0;
            output.bDefaultTargetItemConnected = 0;
            output.bDefaultOverridden = 0;
            output.vDefaultOverrideValue = null;
            output.vSubstituteValue = null;
            output.bEnableSubstituteValue = 0;
            output.szTargetItemPath = null;
            output.szTargetItemName = null;
            output.szSourceServerName = null;
            output.szSourceItemPath = null;
            output.szSourceItemName = null;
            output.dwSourceItemQueueSize = 0;
            output.dwUpdateRate = 0;
            output.fltDeadBand = 0;
            output.szVendorData = null;

            // item name
            if (input.ItemName != null) 
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.ItemName;
                output.szItemName = input.ItemName;
            }

            // item path
            if (input.ItemPath != null) 
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.ItemPath;
                output.szItemPath = input.ItemPath;
            }

            // version
            if (input.Version != null) 
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.Version;
                output.szVersion = input.Version;
            }

            // browse paths
            if (input.BrowsePaths.Count > 0)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.BrowsePaths;
                output.dwBrowsePathCount = input.BrowsePaths.Count;
                output.pszBrowsePaths    = OpcCom.Interop.GetUnicodeStrings(input.BrowsePaths.ToArray());
            }

            // name
            if (input.Name != null)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.Name;
                output.szName = input.Name;
            }

            // description
            if (input.Description != null)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.Description;
                output.szDescription = input.Description;
            }

            // keyword
            if (input.Keyword != null)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.Keyword;
                output.szKeyword = input.Keyword;
            }

            // default source item connected
            if (input.DefaultSourceItemConnectedSpecified)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.DefaultSourceItemConnected;
                output.bDefaultSourceItemConnected = (input.DefaultSourceItemConnected)?1:0;
            }

            // default target item connected
            if (input.DefaultTargetItemConnectedSpecified)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.DefaultTargetItemConnected;
                output.bDefaultTargetItemConnected = (input.DefaultTargetItemConnected)?1:0;
            }

            // default overridden
            if (input.DefaultOverriddenSpecified)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.DefaultOverridden;
                output.bDefaultOverridden = (input.DefaultOverridden)?1:0;
            }

            // default override value
            if (input.DefaultOverrideValue != null)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.DefaultOverrideValue;
                output.vDefaultOverrideValue = input.DefaultOverrideValue;
            }

            // substitute value
            if (input.SubstituteValue != null)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.SubstituteValue;
                output.vSubstituteValue = input.SubstituteValue;
            }

            // enable substitute value
            if (input.EnableSubstituteValueSpecified)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.EnableSubstituteValue;
                output.bEnableSubstituteValue = (input.EnableSubstituteValue)?1:0;
            }

            // target item name
            if (input.TargetItemName != null) 
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.TargetItemName;
                output.szTargetItemName = input.TargetItemName;
            }

            // target item path
            if (input.TargetItemPath != null) 
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.TargetItemPath;
                output.szTargetItemPath = input.TargetItemPath;
            }

            // source server name
            if (input.SourceServerName != null) 
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.SourceServerName;
                output.szSourceServerName = input.SourceServerName;
            }

            // source item name
            if (input.SourceItemName != null) 
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.SourceItemName;
                output.szSourceItemName = input.SourceItemName;
            }

            // source item path
            if (input.SourceItemPath != null) 
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.SourceItemPath;
                output.szSourceItemPath = input.SourceItemPath;
            }

            // source item queue size
            if (input.SourceItemQueueSizeSpecified)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.SourceItemQueueSize;
                output.dwSourceItemQueueSize = input.SourceItemQueueSize;
            }

            // update rate
            if (input.UpdateRateSpecified)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.UpdateRate;
                output.dwUpdateRate = input.UpdateRate;
            }

            // deadband
            if (input.DeadbandSpecified)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.DeadBand;
                output.fltDeadBand = input.Deadband;
            }

            // vendor data
            if (input.VendorData != null)
            {
                output.dwMask |= (uint)OpcRcw.Dx.Mask.VendorData;
                output.szVendorData = input.VendorData;
            }	

            return output;
        }	

        /// <summary>
        /// Converts a COM DXConnection structure to a .NET DXConnection object.
        /// </summary>
        internal static DXConnection GetDXConnection(OpcRcw.Dx.DXConnection input, bool deallocate)
        {			
            DXConnection output = new DXConnection();

            // set output default values.
            output.ItemPath = null;
            output.ItemName = null;
            output.Version = null;
            output.BrowsePaths.Clear();
            output.Name = null;
            output.Description = null;
            output.Keyword = null;
            output.DefaultSourceItemConnected = false;
            output.DefaultSourceItemConnectedSpecified = false;
            output.DefaultTargetItemConnected = false;
            output.DefaultTargetItemConnectedSpecified = false;
            output.DefaultOverridden = false;
            output.DefaultOverriddenSpecified = false;
            output.DefaultOverrideValue = null;
            output.SubstituteValue = null;
            output.EnableSubstituteValue = false;
            output.EnableSubstituteValueSpecified = false;
            output.TargetItemPath = null;
            output.TargetItemName = null;
            output.SourceServerName = null;
            output.SourceItemPath = null;
            output.SourceItemName = null;
            output.SourceItemQueueSize = 0;
            output.SourceItemQueueSizeSpecified = false;
            output.UpdateRate = 0;
            output.UpdateRateSpecified = false;
            output.Deadband = 0;
            output.DeadbandSpecified = false;
            output.VendorData = null;

            // item name
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.ItemName) != 0) 
            {
                output.ItemName = input.szItemName;
            }

            // item path
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.ItemPath) != 0) 
            {
                output.ItemPath = input.szItemPath;
            }

            // version
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.Version) != 0) 
            {
                output.Version = input.szVersion;
            }

            // browse paths
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.BrowsePaths) != 0) 
            {
                string[] browsePaths =  OpcCom.Interop.GetUnicodeStrings(ref input.pszBrowsePaths, input.dwBrowsePathCount, deallocate);
				
                if (browsePaths != null)
                {
                    output.BrowsePaths.AddRange(browsePaths);
                }
            }

            // name
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.Name) != 0) 
            {
                output.Name = input.szName;
            }

            // description
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.Description) != 0) 
            {
                output.Description = input.szDescription;
            }

            // keyword
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.Keyword) != 0) 
            {
                output.Keyword = input.szKeyword;
            }

            // default source item connected
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.DefaultSourceItemConnected) != 0) 
            {
                output.DefaultSourceItemConnected = input.bDefaultSourceItemConnected != 0;
                output.DefaultSourceItemConnectedSpecified = true;
            }

            // default target item connected
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.DefaultTargetItemConnected) != 0) 
            {
                output.DefaultTargetItemConnected = input.bDefaultTargetItemConnected != 0;
                output.DefaultTargetItemConnectedSpecified = true;
            }

            // default overridden
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.DefaultOverridden) != 0) 
            {
                output.DefaultOverridden = input.bDefaultOverridden != 0;
                output.DefaultOverriddenSpecified = true;
            }

            // default override value
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.DefaultOverrideValue) != 0) 
            {
                output.DefaultOverrideValue = input.vDefaultOverrideValue;
            }

            // substitute value
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.SubstituteValue) != 0) 
            {
                output.SubstituteValue = input.vSubstituteValue;
            }

            // enable substitute value
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.EnableSubstituteValue) != 0) 
            {
                output.EnableSubstituteValue = input.bEnableSubstituteValue != 0;
                output.EnableSubstituteValueSpecified = true;
            }

            // target item name
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.TargetItemName) != 0) 
            {
                output.TargetItemName = input.szTargetItemName;
            }

            // target item path
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.TargetItemPath) != 0)
            {
                output.TargetItemPath = input.szTargetItemPath;
            }

            // source server name
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.SourceServerName) != 0)
            {
                output.SourceServerName = input.szSourceServerName;
            }

            // source item name
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.SourceItemName) != 0)
            {
                output.SourceItemName = input.szSourceItemName;
            }

            // source item path
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.SourceItemPath) != 0)
            {
                output.SourceItemPath = input.szSourceItemPath;
            }

            // source item queue size
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.SourceItemQueueSize) != 0)
            {
                output.SourceItemQueueSize = input.dwSourceItemQueueSize;
                output.SourceItemQueueSizeSpecified = true;
            }

            // update rate
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.UpdateRate) != 0)
            {
                output.UpdateRate = input.dwUpdateRate;
                output.UpdateRateSpecified = true;
            }

            // deadband
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.DeadBand) != 0)
            {
                output.Deadband = input.fltDeadBand;
                output.DeadbandSpecified = true;
            }

            // vendor data
            if ((input.dwMask & (uint)OpcRcw.Dx.Mask.VendorData) != 0)
            {
                output.VendorData = input.szVendorData;
            }	

            return output;
        }	

        /// <summary>
        /// Converts an array of .NET ItemIdentifier objects to COM ItemIdentifier structures.
        /// </summary>
        internal static OpcRcw.Dx.ItemIdentifier[] GetItemIdentifiers(Opc.Dx.ItemIdentifier[] input)
        {
            OpcRcw.Dx.ItemIdentifier[] output = null;
			
            if (input != null && input.Length > 0)
            {
                output = new OpcRcw.Dx.ItemIdentifier[input.Length];

                for (int ii = 0; ii < input.Length; ii++)
                {
                    output[ii] = new OpcRcw.Dx.ItemIdentifier();

                    output[ii].szItemName = input[ii].ItemName;
                    output[ii].szItemPath = input[ii].ItemPath;
                    output[ii].szVersion  = input[ii].Version;
                }
            }

            return output;
        }	
        */
    }
}
