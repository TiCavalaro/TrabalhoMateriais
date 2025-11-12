using System;
using Microsoft.EntityFrameworkCore;
namespace ProjetoXico.Data
{

    public static class DbTeste
    {
        public static void TestarConexao(BancoContext context)
        {
            try
            {
                context.Database.OpenConnection();
                Console.WriteLine("Conexão com o banco OK!");

                context.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar ao banco: {ex.Message}");
            }
        }
    }
}
