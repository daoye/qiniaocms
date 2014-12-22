using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    /// <summary>
    /// 长度验证
    /// </summary>
    public class QStringLengthAttribute : StringLengthAttribute
    {
        private QLang lang = QLang.Instance();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maximumLength">最大长度</param>
        public QStringLengthAttribute(int maximumLength)
            : base(maximumLength)
        {

        }

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrWhiteSpace(ErrorMessageResourceName) && ErrorMessageResourceType != null)
            {
                return base.FormatErrorMessage(name);
            }

            return string.Format(lang.Lang(""), name, this.MinimumLength, this.MaximumLength);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule modelClientValidationRole = new ModelClientValidationRule()
            {
                ValidationType = "qstringlength",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };

            modelClientValidationRole.ValidationParameters.Add("min", MinimumLength);
            modelClientValidationRole.ValidationParameters.Add("max", MaximumLength);

            yield return modelClientValidationRole;
        }
    }
}
