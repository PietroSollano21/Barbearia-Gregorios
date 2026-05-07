namespace Barbearia.Models
{
    public class PagamentoViewModel
    {

        public long AgendamentoId { get; set; }
        public decimal Valor { get; set; }
        public string QrCode { get; set; }
        public string QrCodeBase64 { get; set; }
        public string CopiaECola { get; set; }

    }
}