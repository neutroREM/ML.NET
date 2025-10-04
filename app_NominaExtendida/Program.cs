namespace app_NominaExtendida
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var orquestador = new cls_OrquestadorServicio();
            await orquestador.IniciarServicio();
        }
    }
}