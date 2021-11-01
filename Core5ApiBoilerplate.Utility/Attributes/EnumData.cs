using System;

namespace Core5ApiBoilerplate.Utility.Attributes
{
    public class EnumData : Attribute
    {
        public EnumData(object data)
        {
            Data = data;
        }

        public object Data { get; set; }
    }
}
