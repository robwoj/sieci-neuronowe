using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perceptron1
{
    /// <summary>
    /// Klasa reprezentująca argumenty zdarzenia LayerEvent
    /// </summary>
    public class LayerEventArgs : EventArgs
    {
        private UniqueLayer layer;

        public UniqueLayer Layer
        {
            get
            {
                return layer;
            }
        }

        public LayerEventArgs(UniqueLayer lay)
        {
            layer = lay;
        }
    }

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
    /// Klasa reprezentująca argumenty zdarzenia NetworkEvent
    /// </summary>
    public class EventArgs : EventArgs
    {
        private MLPNetwork network;
        public MLPNetwork Network
        {
            get
            {
                return network;
            }
        }

        public EventArgs(MLPNetwork net)
        {
            network = net;
        }
    }
}
