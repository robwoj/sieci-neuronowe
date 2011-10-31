namespace Perceptron1
{
    /// <summary>
    /// Delegacja odpowiadająca zdarzeniu utworzenia nowej sieci neuronowej
    /// </summary>
    public delegate void NetworkEvent(object sender, EventArgs e);

    /// <summary>
    /// Delegacja odpowiadająca zdarzeniu utworzenia i nauczenia perceptronu
    /// </summary>
    public delegate void PerceptronEvent(object sender, PerceptronEventArgs e);

    /// <summary>
    /// Delegacja odpowiadająca zdarzeniu utworzenia nowej warstwy
    /// </summary>
    public delegate void LayerEvent(object sender, LayerEventArgs e);    
}