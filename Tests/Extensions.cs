using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	public static class Extensions
	{
		public static bool InheritsFrom(this Type thisBaseType, Type baseType)
		{
			bool rtnValue = false;

			if (baseType == null)
			{
				// Since baseType isn't provided ignore check and return true
				rtnValue = true;
			}
			else
			{
				if (thisBaseType == baseType || thisBaseType.BaseType == baseType)
				{
					rtnValue = true;
				}
				else
				{
					// Check if base type is further down the inheritance list
					if (thisBaseType.BaseType.BaseType != null)
					{
						rtnValue = thisBaseType.BaseType.InheritsFrom(baseType);
					}
				}
			}

			return rtnValue;
		}
	}
}
