using System;
using System.Data.Entity.Validation;
using System.Text;
using Elmah;
using ElmahExtensions;

namespace Domain.Common
{
    public static class ElmahExtensions
    {
        public static ErrorLogEntry OurRaise(this CustomErrorSignal signal, Exception ex)
        {
            if (!(ex is DbEntityValidationException)) return signal.Raise(ex);

            var e = ex as DbEntityValidationException;

            var errorStringBilder = new StringBuilder();

            foreach (var entityValidationError in e.EntityValidationErrors)
            {
                errorStringBilder.Append(
                    string.Format("Entity \"{0}\" in state \"{1}\", errors:",
                        entityValidationError.Entry.Entity.GetType().Name,
                        entityValidationError.Entry.State));

                foreach (var error in entityValidationError.ValidationErrors)
                {
                    errorStringBilder.Append(
                        string.Format(" (Property: \"{0}\", Error: \"{1}\")",
                            error.PropertyName, error.ErrorMessage));
                }
            }

            e.Data.Add("EntityValidationErrors", errorStringBilder.ToString());
            return signal.Raise(ex);
        }
    }
}