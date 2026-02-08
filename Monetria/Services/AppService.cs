namespace Monetria.Services
{
    public static class AppServices
    {
        public static TransacaoService TransacaoService { get; } = new();
        public static ThemeService ThemeService { get; } = new();
    }
}