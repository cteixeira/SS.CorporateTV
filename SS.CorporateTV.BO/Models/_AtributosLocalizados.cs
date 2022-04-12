using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SS.CorporateTV.BO.Models
{
    public class DateGreaterThan : ValidationAttribute, IClientValidatable
    {
        public DateGreaterThan(Type resourceType, string myprop, string otherprop)
        {
            _myproplocalizado = resourceType.GetProperty(myprop, BindingFlags.Public | BindingFlags.Static).GetValue(myprop, null).ToString();
            _otherproplocalizado = resourceType.GetProperty(otherprop, BindingFlags.Public | BindingFlags.Static).GetValue(otherprop, null).ToString();

            MyProperty = myprop;
            OtherProperty = otherprop;
        }

        private readonly string _myproplocalizado;
        private readonly string _otherproplocalizado;
        public string MyProperty { get; set; }
        public string OtherProperty { get; set; }

        public string FormatErrorMessage(string name, string otherName)
        {
            return string.Format(ErrorMessageString, name, otherName);
        }

        protected override ValidationResult
            IsValid(object firstValue, ValidationContext validationContext)
        {
            var firstComparable = firstValue as IComparable;
            var secondComparable = GetSecondComparable(validationContext);

            if (firstComparable != null && secondComparable != null)
            {
                if (firstComparable.CompareTo(secondComparable) < 0)
                {
                    object obj = validationContext.ObjectInstance;
                    var thing = obj.GetType().GetProperty(OtherProperty);

                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName, thing.Name));
                }
            }

            return ValidationResult.Success;
        }

        protected IComparable GetSecondComparable(
            ValidationContext validationContext)
        {
            var propertyInfo = validationContext
                                  .ObjectType
                                  .GetProperty(OtherProperty);
            if (propertyInfo != null)
            {
                var secondValue = propertyInfo.GetValue(
                    validationContext.ObjectInstance, null);
                return secondValue as IComparable;
            }
            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ValidationParameters["startdate"] = MyProperty;
            rule.ValidationParameters["enddate"] = OtherProperty;
            rule.ValidationType = "dategreaterthan";
            rule.ErrorMessage = String.Format(Resources.Erro.HoraDeveSerInferiorA, _myproplocalizado, _otherproplocalizado);
            return new[] { rule };
        }
    }

    public class TimeGreaterThan : ValidationAttribute, IClientValidatable
    {
        public TimeGreaterThan(Type resourceType, string myprop, string otherprop)
        {

            _myproplocalizado = resourceType.GetProperty(myprop, BindingFlags.Public | BindingFlags.Static).GetValue(myprop, null).ToString();
            _otherproplocalizado = resourceType.GetProperty(otherprop, BindingFlags.Public | BindingFlags.Static).GetValue(otherprop, null).ToString();
            //_myproplocalizado = myprop;
            //_otherproplocalizado = otherprop;

            MyProperty = myprop;
            OtherProperty = otherprop;
        }

        private readonly string _myproplocalizado;
        private readonly string _otherproplocalizado;
        public string MyProperty { get; set; }
        public string OtherProperty { get; set; }

        public string FormatErrorMessage(string name, string otherName)
        {
            return string.Format(ErrorMessageString, name, otherName);
        }

        protected override ValidationResult
            IsValid(object firstValue, ValidationContext validationContext)
        {
            var firstComparable = firstValue as IComparable;
            var secondComparable = GetSecondComparable(validationContext);

            if (firstComparable != null && secondComparable != null)
            {
                if (firstComparable.CompareTo(secondComparable) < 1)
                {
                    object obj = validationContext.ObjectInstance;
                    var thing = obj.GetType().GetProperty(OtherProperty);

                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName, thing.Name));
                }
            }

            return ValidationResult.Success;
        }

        protected IComparable GetSecondComparable(
            ValidationContext validationContext)
        {
            var propertyInfo = validationContext
                                  .ObjectType
                                  .GetProperty(OtherProperty);
            if (propertyInfo != null)
            {
                var secondValue = propertyInfo.GetValue(
                    validationContext.ObjectInstance, null);
                return secondValue as IComparable;
            }
            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ValidationParameters["startdate"] = MyProperty;
            rule.ValidationParameters["enddate"] = OtherProperty;
            rule.ValidationType = "timegreaterthan";
            rule.ErrorMessage = String.Format(Resources.Erro.HoraDeveSerInferiorA, _myproplocalizado, _otherproplocalizado);
            return new[] { rule };
        }
    }


    public class TimeSpanValidationAttribute : ValidationAttribute, IClientValidatable
    {
        public bool IsValid()
        {
            return true;
            // Your IsValid() implementation here
        }

        // IClientValidatable implementation
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new TimeSpanValidationRule("Please specify a valid timespan (hh:mm)", "hh:mm");
            yield return rule;
        }

        public class TimeSpanValidationRule : ModelClientValidationRule
        {
            public TimeSpanValidationRule(string error, string format)
            {
                ErrorMessage = error;
                ValidationType = "timespan";
                ValidationParameters.Add("format", format);
            }
        }
    }

    public class DisplayLocalizado : DisplayNameAttribute
    {
        private PropertyInfo propInfo;

        public DisplayLocalizado(Type resourceType, string resourceKey)
            : base(resourceKey)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        public override string DisplayName
        {
            get
            {
                return propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            }
        }
    }

    public class RequiredLocalizado : RequiredAttribute, IClientValidatable
    {
        private PropertyInfo propInfo;

        public RequiredLocalizado(Type resourceType, string resourceKey)
            : base()
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.CampoObrigatorio, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "required";
            return new[] { rule };
        }
    }

    public class MaxStringLocalizado : StringLengthAttribute, IClientValidatable
    {
        private PropertyInfo propInfo;

        public MaxStringLocalizado(int maxLenght, Type resourceType, string resourceKey)
            : base(maxLenght)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.TamanhoMaximo, nomeCampo, this.MaximumLength.ToString());

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "length";
            rule.ValidationParameters.Add("max", this.MaximumLength);
            return new[] { rule };
        }
    }

    public class FormatoEmailLocalizado : RegularExpressionAttribute, IClientValidatable
    {
        private const string pattern = @"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2,6})?)$";
        private PropertyInfo propInfo;

        public FormatoEmailLocalizado(Type resourceType, string resourceKey)
            : base(pattern)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.FormatoEmail, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }

    public class FormatoNumeroDecimalLocalizado : DataTypeAttribute, IClientValidatable
    {
        private const string pattern = @"^\d+([,.]\d+)?";
        private PropertyInfo propInfo;

        public FormatoNumeroDecimalLocalizado(Type resourceType, string resourceKey)
            : base(pattern)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.FormatoNumeroDecimal, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }

    public class FormatoIdentificadorLocalizado : DataTypeAttribute, IClientValidatable
    {
        private const string pattern = @"^\d+$";
        private PropertyInfo propInfo;

        public FormatoIdentificadorLocalizado(Type resourceType, string resourceKey)
            : base(pattern)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();

            this.ErrorMessage = String.Format(Resources.Erro.CampoObrigatorio, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }

    public class FormatoNumeroInteiroLocalizado : DataTypeAttribute, IClientValidatable
    {
        private const string pattern = @"^(\+|-)?\d+$";
        private PropertyInfo propInfo;

        public FormatoNumeroInteiroLocalizado(Type resourceType, string resourceKey)
            : base(pattern)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);

        }

        public FormatoNumeroInteiroLocalizado(Type resourceType, string resourceKey, string errorMessage)
            : base(pattern)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
            ErrorMessage = errorMessage.ToString();
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();

            if (string.IsNullOrEmpty(this.ErrorMessage))
                this.ErrorMessage = String.Format(Resources.Erro.FormatoNumeroInteiro, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }

    public class FormatoDataLocalizado : DataTypeAttribute, IClientValidatable
    {
        private const string pattern = @"^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$";
        private PropertyInfo propInfo;

        public FormatoDataLocalizado(Type resourceType, string resourceKey)
            : base(pattern)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.FormatoData, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }

    public class FormatoHoraLocalizado : DataTypeAttribute, IClientValidatable
    {
        private const string pattern = @"^(20|21|22|23|[01]\d|\d)(([:.][0-5]\d){1,2})$";
        private PropertyInfo propInfo;

        public FormatoHoraLocalizado(Type resourceType, string resourceKey)
            : base(pattern)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            this.ErrorMessage = String.Format(Resources.Erro.FormatoHora, nomeCampo);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "regex";
            rule.ValidationParameters.Add("pattern", pattern);
            return new[] { rule };
        }
    }

    public class RangeLocalizado : RangeAttribute, IClientValidatable
    {
        private PropertyInfo propInfo;

        public RangeLocalizado(int minValue, int maxValue, Type resourceType, string resourceKey)
            : base(minValue, maxValue)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            if ((Int32)this.Minimum == Int32.MinValue && (Int32)this.Maximum < Int32.MaxValue)
                this.ErrorMessage = String.Format(Resources.Erro.NumeroMenorQue, nomeCampo, this.Maximum);
            else if ((Int32)this.Minimum > Int32.MinValue && (Int32)this.Maximum == Int32.MaxValue)
                this.ErrorMessage = String.Format(Resources.Erro.NumeroMaiorQue, nomeCampo, this.Minimum);
            else
                this.ErrorMessage = String.Format(Resources.Erro.NumeroEntre, nomeCampo, this.Minimum, this.Maximum);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "range";
            rule.ValidationParameters.Add("min", this.Minimum);
            rule.ValidationParameters.Add("max", this.Maximum);
            return new[] { rule };
        }
    }

    public class RangeDoubleLocalizado : RangeAttribute, IClientValidatable
    {
        private PropertyInfo propInfo;

        public RangeDoubleLocalizado(double minValue, double maxValue, Type resourceType, string resourceKey)
            : base(minValue, maxValue)
        {
            propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string nomeCampo = propInfo.GetValue(propInfo.DeclaringType, null).ToString();
            if ((Double)this.Minimum == Double.MinValue && (Double)this.Maximum < Double.MaxValue)
                this.ErrorMessage = String.Format(Resources.Erro.NumeroMenorQue, nomeCampo, this.Maximum);
            else if ((Double)this.Minimum > Double.MinValue && (Double)this.Maximum == Double.MaxValue)
                this.ErrorMessage = String.Format(Resources.Erro.NumeroMaiorQue, nomeCampo, this.Minimum);
            else
                this.ErrorMessage = String.Format(Resources.Erro.NumeroEntre, nomeCampo, this.Minimum, this.Maximum);

            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = this.ErrorMessage;
            rule.ValidationType = "rangedouble";
            rule.ValidationParameters.Add("min", this.Minimum);
            rule.ValidationParameters.Add("max", this.Maximum);
            return new[] { rule };
        }
    }

    public class RequiredIfLocalizado : ValidationAttribute, IClientValidatable
    {
        private PropertyInfo propInfo;

        public string dependentProperty { get; set; }

        public RequiredIfLocalizado(Type resourceType, string resourceKey, string dependentProperty)
        {
            this.dependentProperty = dependentProperty;
            this.propInfo = resourceType.GetProperty(resourceKey, BindingFlags.Public | BindingFlags.Static);
        }

        //Necessário para despoletar a validação no cliente e a definição da mensagem de erro localizada
        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = String.Format(Resources.Erro.CampoObrigatorio, propInfo.GetValue(propInfo.DeclaringType, null).ToString()),
                ValidationType = "requiredif",
            };

            rule.ValidationParameters.Add("dependent", this.dependentProperty);

            yield return rule;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var containerType = validationContext.ObjectInstance.GetType();
            var field = containerType.GetProperty(this.dependentProperty);

            //Para os casos em que o campo de que se depende é um calendário e o id começa por "cal"
            if (field == null && this.dependentProperty.ToLower().StartsWith("cal"))
            {
                field = containerType.GetProperty(this.dependentProperty.Remove(0, 3));
            }

            if (field != null)
            {
                //Obtém o valor do campo de que se depende
                var dependentValue = field.GetValue(validationContext.ObjectInstance, null);

                //Verifica se o valor é uma string, se for faz trim
                bool isEmptyString = false;
                if (dependentValue != null && dependentValue is string)
                {
                    dependentValue = (dependentValue as string).Trim();
                    isEmptyString = string.IsNullOrEmpty(dependentValue as string);
                }

                if (dependentValue != null && !isEmptyString)
                {
                    //Verifica se o campo dependente é válido
                    if (!new RequiredAttribute().IsValid(value))
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AtLeastOneRequired : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            var typeInfo = value.GetType();

            var propertyInfo = typeInfo.GetProperties();

            foreach (var property in propertyInfo)
            {
                if (null != property.GetValue(value, null))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class ExportExcel : Attribute
    {

        public enum ExportType
        {
            String,
            Numeric,
            DateOnly,
            TimeOnly,
            DateTime
        }

        public bool ExportToExcel { get; set; }

        public ExportType Type { get; set; }

        public int? ExportOrder { get; set; }

        public ExportExcel(bool ExportToExcel) :
            this(ExportToExcel, ExportType.String, null)
        {
        }

        public ExportExcel(bool ExportToExcel, int ExportOrder) :
            this(ExportToExcel, ExportType.String, ExportOrder)
        {
        }

        public ExportExcel(bool ExportToExcel, ExportType Type) :
            this(ExportToExcel, Type, null)
        {
        }

        public ExportExcel(bool ExportToExcel, ExportType Type, int? ExportOrder)
        {
            this.ExportToExcel = ExportToExcel;
            this.Type = Type;
            this.ExportOrder = ExportOrder;
        }

    }
}