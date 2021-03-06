﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MLPNetworkLib
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
    /// Klasa reprezentująca argumenty zdarzenia NetworkEvent
    /// </summary>
    public class NetworkEventArgs : EventArgs
    {
        private MLPNetwork network;
        public MLPNetwork Network
        {
            get
            {
                return network;
            }
        }

        public NetworkEventArgs(MLPNetwork net)
        {
            network = net;
        }
    }

    /// <summary>
    /// Klasa reprezentująca zdarzenie zakończenia iteracji algorytmu
    /// </summary>
    public class NetworkLearningIterationEventArgs : NetworkEventArgs
    {
        private int iterationNumber;

        public int IterationNumber
        {
            get
            {
                return iterationNumber;
            }
        }


        public NetworkLearningIterationEventArgs(MLPNetwork network, int iteration)
            : base(network)
        {
            iterationNumber = iteration;
        }
    }

    public delegate void NetworkEvent(object sender, NetworkEventArgs e);
    public delegate void LayerEvent(object sender, LayerEventArgs e);
}
