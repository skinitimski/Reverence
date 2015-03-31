using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using NLua;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal abstract class Equipment : IInventoryItem
    {
        protected Equipment()
        {
            Name = String.Empty;
            Desc = String.Empty;
            
            DoAttach = (LuaFunction)Seven.Lua.DoString("return function (c) end").First();
            DoDetach = (LuaFunction)Seven.Lua.DoString("return function (c) end").First();
        }

        protected Equipment(XmlNode node)
            : this()
        {     
            Name = node.SelectSingleNode("name").InnerText;
            Desc = node.SelectSingleNode("desc").InnerText;
                        
            XmlNode attachNode = node.SelectSingleNode("attach");

            if (attachNode != null)
            {
                string attach = node.SelectSingleNode("attach").InnerText;
                string attachFunction = String.Format("return function (c) {0} end", attach);

                try
                {
                    DoAttach = (LuaFunction)Seven.Lua.DoString(attachFunction).First();
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in equipment attach script; id = " + ID, e);
                }
            }

            XmlNode detachNode = node.SelectSingleNode("detach");

            if (detachNode != null)
            {
                string detach = node.SelectSingleNode("detach").InnerText;
                string detachFunction = String.Format("return function (c) {0} end", detach);

                try
                {
                    DoDetach = (LuaFunction)Seven.Lua.DoString(detachFunction).First();
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in equipment detach script; id = " + ID, e);
                }
            }
        }
        
        public void Attach(Character c)
        {
            try
            {
                DoAttach.Call(c);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error calling equipment attach script; id = " + ID, e);
            }
        }
        public void Detach(Character c)
        {
            try
            {
                DoDetach.Call(c);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error calling equipment detach script; id = " + ID, e);
            }
        }
        
        public string Name { get; private set; }
        
        public string ID { get { return Resource.CreateID(Name); } }

        protected string Desc { get; private set; }
        
        public virtual string Description { get { return Desc; } }
        
        private LuaFunction DoAttach { get; set; }
        
        private LuaFunction DoDetach { get; set; }
        
        public bool CanUseInField { get { return false; } }
        
        public bool CanUseInBattle { get { return false; } }
    }
}

