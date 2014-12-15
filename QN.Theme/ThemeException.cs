#region
/********************************************************************
	This is NOT a freeware, use is subject to license terms.
	
	Copyright:	QiNiaoSoft Copyright (C) 2014 All rights Reserved.
	Created:	2014/12/06
	FileName: 	ThemeException.cs
	Author:		DaoYe
	History: 	6/12/2014 14:17 By DaoYe
	Purpose:	
*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Theme
{
    public class ThemeException : QRunException
    {
        public ThemeException(string message)
            : base(message)
        { }
    }
}