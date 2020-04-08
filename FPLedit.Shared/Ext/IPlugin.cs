﻿namespace FPLedit.Shared
{
    /// <summary>
    /// Base interface for all extensions.
    /// </summary>
    /// <remarks>All external inheritors must have a <see cref="PluginAttribute"/> to be discovered by FPLedit.</remarks>
    public interface IPlugin
    {
        void Init(IPluginInterface pluginInterface, IComponentRegistry componentRegistry);
    }
}
