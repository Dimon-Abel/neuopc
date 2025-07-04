/*
ScPl - A plotting library for .NET

ArrayAdapterDT.cs
Copyright (C) 2003
Matt Howlett

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
   
2. Redistributions in binary form must reproduce the following text in 
   the documentation and / or other materials provided with the 
   distribution: 
   
   "This product includes software developed as part of 
   the ScPl plotting library project available from: 
   http://www.netcontrols.org/scpl/" 

------------------------------------------------------------------------

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

$Id: ArrayAdapterDT.cs,v 1.3 2003/11/23 02:05:43 mhowlett Exp $

*/

using System;

// THIS DOESN'T WORK YET.

namespace scpl
{
	/// <summary>
	/// Summary description for ArrayAdapterDT.
	/// </summary>
	public class ArrayAdapterDT : ISequenceAdapter
	{
		public ArrayAdapterDT( double[] ys, DateTime start, DateTime step )
		{
			
		}

		// also provide a special method that time axis looks for using reflection.
		public PointD this[int i]
		{
			get
			{
				return new PointD(0.0,0.0);
			}
		}

		public int Count
		{
			get
			{
				return 0;
			}
		}

		public Axis SuggestXAxis()
		{
			return new TimeAxis();
		}

		public Axis SuggestYAxis()
		{
			return new LinearAxis();
		}

	}
}
