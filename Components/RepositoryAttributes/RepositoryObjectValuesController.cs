using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2005
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Modules.Repository
{

	#region "RepositoryObjectValues"

	public class RepositoryObjectValuesController
	{

		public ArrayList GetRepositoryObjectValues(int objectID)
		{
			return CBO.FillCollection(DataProvider.Instance().GetRepositoryObjectValues(objectID), typeof(RepositoryObjectValuesInfo));
		}
		public RepositoryObjectValuesInfo GetSingleRepositoryObjectValues(int objectID, int valueId)
		{
            return CBO.FillObject<RepositoryObjectValuesInfo>(DataProvider.Instance().GetSingleRepositoryObjectValues(objectID, valueId));
		}
		public int AddRepositoryObjectValues(RepositoryObjectValuesInfo RepositoryObjectValuesInfo)
		{
			return Convert.ToInt32(DataProvider.Instance().AddRepositoryObjectValues(RepositoryObjectValuesInfo.ObjectID, RepositoryObjectValuesInfo.ValueID));
		}
		public static void UpdateRepositoryObjectValues(RepositoryObjectValuesInfo RepositoryObjectValuesInfo)
		{
			DataProvider.Instance().UpdateRepositoryObjectValues(RepositoryObjectValuesInfo.ItemID, RepositoryObjectValuesInfo.ObjectID, RepositoryObjectValuesInfo.ValueID);
		}
		public static void DeleteRepositoryObjectValues(int objectID)
		{
			DataProvider.Instance().DeleteRepositoryObjectValues(objectID);
		}

	}

	#endregion

}

