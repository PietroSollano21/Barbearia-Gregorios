namespace Barbearia.Models
{
    public class PagamentoViewModel
    {

        public long AgendamentoId { get; set; }
        public decimal Valor { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; }= string.Empty;
        public string CopiaECola { get; set; } = string.Empty;

    }
}