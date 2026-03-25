using Barbearia.Models;

namespace  Barbearia.Services
{
   public class AgendamentoService
   {
    public void CalcularValor(Agendamento agendamento)
    {
        switch (agendamento.Corte)
            {
                case "Degradê":
                    agendamento.Valor = 30.00m;
                    break;
                case "Social":
                    agendamento.Valor = 25.00m;
                    break;
                case "Corte e Barba":
                    agendamento.Valor = 35.00m;
                    break;
                default:
                    agendamento.Valor = 0.00m;
                    break;
            }
    }
   }
}