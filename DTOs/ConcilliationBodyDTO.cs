using System.ComponentModel.DataAnnotations;

namespace ConcilliationConsumer.Dtos;

public class ConcilliationBodyDTO(long paymentProviderId, string date, string file, string postback)
{
  [Required(ErrorMessage = "Field paymentProviderId is required")]
  public long PaymentProviderId { get; } = paymentProviderId;
  
  [Required(ErrorMessage = "Field date is required")]
  public string Date { get; } = date;

  [Required(ErrorMessage = "Field file is required")]
  public string File { get; } = file;

  [Required(ErrorMessage = "Field postback is required")]
  public string Postback { get; } = postback;
}