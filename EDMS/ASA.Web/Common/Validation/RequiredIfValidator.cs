using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ASA.Web.Common.Validation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private RequiredAttribute attributeToRequire = new RequiredAttribute();
        public string DependentProperty { get; set; }
        public string DependsOn { get; set; }

        public RequiredIfAttribute(string dependentProperty, string dependsOn)
        {
            DependentProperty = dependentProperty;
            DependsOn = dependsOn;
        }

        public override bool IsValid(object value)
        {

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object propertyToCheckForPopulation = null;

            if (properties != null)
            {
                PropertyDescriptor pd = properties.Find(DependsOn, true);
                if (pd != null)
                {
                    propertyToCheckForPopulation = pd.GetValue(value);
                }

                if (propertyToCheckForPopulation != null)
                {
                    PropertyDescriptor pdDependent = properties.Find(DependentProperty, true);
                    if (pdDependent != null)
                        return attributeToRequire.IsValid(pdDependent.GetValue(value));
                    else
                        return false;
                }
                else
                    return true;
            }
            else
                return true;
        } 

        private object _typeId = new object();
        public override object TypeId
        {
            get
            {
                return this._typeId;
            }
        }


    }


}
