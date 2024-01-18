﻿namespace Niculae_Ana_Maria_Proiect4.Models
{
    public class Comentariu
    {
        public int ComentariuId { get; set; }
        public string Text { get; set; }
        public DateTime DataOra { get; set; }

        // Cheie străină pentru utilizator (manager sau membru echipă)
        public int AutorId { get; set; }
        //public Utilizator Autor { get; set; } // Proprietatea de navigație către utilizator


        public int? SarcinaId { get; set; }
        public Sarcina Sarcina { get; set; }

        // Constructorul implicit

    }
}
