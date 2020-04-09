﻿using System.Collections.Generic;
using Force.DeepCloner;

namespace FPLedit.Shared
{
    /// <summary>
    /// Base type for all entities that are used in a timetable object model. Provides basic capabilities foir storing
    /// attributes, children & interacting with underlying <see cref="XMLEntity"/>s.
    /// </summary>
    /// <remarks>All inheriting classes MUST implement the constructor <see cref="Entity(FPLedit.Shared.XMLEntity,FPLedit.Shared.Timetable)"/> itself!</remarks>
    [Templating.TemplateSafe]
    public abstract class Entity : IEntity
    {
        public XMLEntity XMLEntity { get; }

        public Timetable _parent { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> Attributes
        {
            get => XMLEntity.Attributes;
            set => XMLEntity.Attributes = value;
        }

        /// <inheritdoc />
        public List<XMLEntity> Children => XMLEntity.Children;

        /// <summary>
        /// Creates a new instance of this entity type, specifying the XML node name.
        /// </summary>
        /// <param name="xn">XML node name for the newly constructed underlying <see cref="XMLEntity" />.</param>
        /// <param name="tt">A reference to the parent timetable.</param>
        protected Entity(string xn, Timetable tt)
        {
            XMLEntity = new XMLEntity(xn);
            _parent = tt;
        }

        /// <summary>
        /// Creates a new instance of this entity type, based on the given <see cref="XMLEntity"/>.
        /// </summary>
        /// <param name="en">The already-initialized XML Entity.</param>
        /// <param name="tt">A reference to the parent timetable.</param>
        /// <remarks>The XML entity must already be registered in the XML tree, if ist is not the top-level element itself.</remarks>
        protected Entity(XMLEntity en, Timetable tt)
        {
            XMLEntity = en;
            _parent = tt;
        }

        /// <inheritdoc />
        public T GetAttribute<T>(string key, T defaultValue = default)
            => XMLEntity.GetAttribute<T>(key, defaultValue);

        /// <inheritdoc />
        public void SetAttribute(string key, string value)
            => XMLEntity.SetAttribute(key, value);

        /// <inheritdoc />
        public void RemoveAttribute(string key)
            => XMLEntity.RemoveAttribute(key);

        /// <summary>
        /// Create a deep clone of the current entity object.
        /// </summary>
        /// <typeparam name="T">Type of the current entity, needs to be specified.</typeparam>
        public T Clone<T>() where T : Entity => (T)this.DeepClone();

        protected void SetNotEmptyTimeAttribute(string key, TimeEntry time)
        {
            var t = time.ToShortTimeString();
            SetAttribute(key, t != "00:00" ? t : "");
        }

        protected TimeEntry GetTimeAttributeValue(string key)
        {
            var val = GetAttribute(key, "");
            TimeEntry.TryParse(val, out var ts);
            return ts;
        }
    }
}
