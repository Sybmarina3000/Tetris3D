﻿using System;
using System.Collections.Generic;

namespace Helper.Patterns.Factory
{
    public class Factory<Base> where Base : class
    {
        private Dictionary<string, AbstractCreator<Base>> _creators;

        public Factory()
        {
            _creators = new Dictionary<string, AbstractCreator<Base>>();
        }

        public void AddCreator<T>(string type) where T : Base, new()
        {
            _creators.Add(type, new ConcreteCreator<Base, T>());
        }

        public Base Create(string type)
        {
            return _creators[type].Create();
        }
    }
}