using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace QN
{
    public class QRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        private QLang lang = QLang.Instance();

        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrWhiteSpace(ErrorMessageResourceName) && ErrorMessageResourceType != null)
            {
                return base.FormatErrorMessage(name);
            }

            if(string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "必须填写";
            }

            return string.Format(lang.Lang(ErrorMessage), lang.Lang(name));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule modelClientValidationRole = new ModelClientValidationRule()
            {
                ValidationType = "qrequired",
                ErrorMessage = FormatErrorMessage(lang.Lang(metadata.DisplayName))
            };

            yield return modelClientValidationRole;
        }
    }
}