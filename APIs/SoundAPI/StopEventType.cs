namespace SoundAPI
{
    /// <summary>
    /// Type of event a custom stop audio event will be added to.
    /// </summary>
    public enum StopEventType
    {
        /// <summary>
        /// The custom stop audio event will not be added to any special global stop audio events.
        /// </summary>
        None,
        /// <summary>
        /// The custom stop audio event will be added to the "Stop_MUS_All" event.
        /// </summary>
        Music,
        /// <summary>
        /// The custom stop audio event will be added to the "Stop_WPN_All" event.
        /// </summary>
        Weapon,
        /// <summary>
        /// The custom stop audio event will be added to the "Stop_SND_OBJ" event.
        /// </summary>
        Object
    }
}
