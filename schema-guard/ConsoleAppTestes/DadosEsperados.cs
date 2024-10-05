using System;
using System.Collections.Generic;
using System.Text;

namespace schema_guard
{
    public class Data
    {
        public string texto_status_execucao { get; set; }
        public string data_hora_evento { get; set; }
        public string descricao_status_operacao { get; set; }
        public string descricao_contrato_operacao { get; set; }
        public string codigo_identificador_boleto { get; set; }
    }

    public class Value
    {
        public Data data { get; set; }
    }

    public class Payload
    {
        public string key { get; set; }
        public Value value { get; set; }
        public string topic { get; set; }
        public int partition { get; set; }
        public int offset { get; set; }
        public long timestamp { get; set; }
        public List<Header> headers { get; set; }
    }

    public class Header
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class Message
    {
        public Payload payload { get; set; }
    }

    public class Root
    {
        public string date { get; set; }
        public string guid { get; set; }
        public string level { get; set; }
        public List<Message> message { get; set; }
    }

}
