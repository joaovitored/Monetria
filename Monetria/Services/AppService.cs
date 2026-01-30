
namespace Monetria.Services
{
    public static class AppServices
    {
        // Serviço único compartilhado
        public static TransacaoService TransacaoService { get; } = new();
    }
}