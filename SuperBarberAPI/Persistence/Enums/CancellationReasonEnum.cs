using System.ComponentModel.DataAnnotations;

namespace Persistence.Enums
{
    public enum CancellationReasonEnum
    {
        [Display(Name = "Няма връзка с клиента")]
        NoCustomerConnection,
        [Display(Name = "Клиентът не дойде")]
        CustomerDidNotCome,
        [Display(Name = "Отменен от клиента")]
        CanceledByTheCustomer,
        [Display(Name = "Отменен от салона")]
        CanceledByTheSalon,
        [Display(Name = "Променен от клиента")]
        ChangedByTheCustomer,
        [Display(Name = "Променен от салона")]
        ChangedByTheSalon
    }
}
