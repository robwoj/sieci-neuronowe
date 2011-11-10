using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerceptronLib
{
    /// <summary>
    /// Klasa reprezentująca argumenty zdarzenia PerceptronEvent
    /// </summary>
    public class PerceptronEventArgs : EventArgs
    {
        private Perceptron perceptron;

        public Perceptron Perceptron
        {
            get
            {
                return perceptron;
            }
        }

        public PerceptronEventArgs(Perceptron p)
        {
            perceptron = p;
        }
    }
    
    /// <summary>
    /// Szkielet zdarzenia dotyczącego perceptronu
    /// </summary>
    public delegate void PerceptronEvent(object sender, PerceptronEventArgs e);

}
