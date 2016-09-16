/*
 * Belong
 * 2016-09-15
*/

using System;

namespace Belong.BehaviorTree
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class BFieldAttribute : Attribute
    {
        private string _showName = string.Empty;
        public string ShowName
        {
            get
            {
                return _showName;
            }
            set
            {
                _showName = value;
            }
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class BHideFieldAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BClassAttribute : Attribute
    {
        private string _showName = string.Empty;
        public string ShowName
        {
            get
            {
                return _showName;
            }
            set
            {
                _showName = value;
            }
        }
    }
}
