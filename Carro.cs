using System;

namespace desafioford
{
    public class Carro
    {
        public int id_carro { get; set; }
        public string Placa { get; set; }
        public string Cor { get; set; }
        public string Data { get; set; }
        public Carro(string placa, string cor=null)
        {
            this.Placa = placa;
            this.Cor = cor;
        }
    }
}