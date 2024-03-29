using System.Collections.Generic;
using System.Dynamic;

namespace ScourgeBloom
{
    /// <summary>
    ///     class to dynamically keep Sequence/Selector content info
    ///     .. without having to define a new class everywhere
    /// </summary>
    public class DynamicContext : DynamicObject
    {
        // The inner dictionary.
        private readonly Dictionary<string, object> _dictionary
            = new Dictionary<string, object>();

        // number of properties defined in instance
        public int Count => _dictionary.Count;

        // If you try to get a value of a property
        // not defined in the class, this method is called.
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            var name = binder.Name.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return _dictionary.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            _dictionary[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }
}