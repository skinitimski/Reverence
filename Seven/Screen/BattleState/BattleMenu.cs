using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using SummonSpell = Atmosphere.Reverence.Seven.Asset.Summon;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class BattleMenu : ControlMenu, ISelectorUser
    {
        #region Layout

        const int x = 35;
        const int y = 30;
        const int y0 = 5;
        const int cx = 20;
        const int cy = 28;
        private readonly int xs = Seven.Config.WindowWidth / 5; // W

        #endregion Layout

        private int _attackOption = -1;
        private int _doubleCutOption2 = -1;
        private int _doubleCutOption4 = -1;
        private int _slashAllOption = -1;
        private int _flashOption = -1;
        private int _magicMenuOption = -1;
        private int _summonMenuOption = -1;
        private int _senseOption = -1;
        private int _mimeOption = -1;
        private int _deathblowOption = -1;
        private int _stealOption = -1;
        private int _mugOption = -1;
        private int _enemySkillMenuOption = -1;
        private int _itemMenuOption = 3;
        private bool _witem = false;
        private bool _wmagic = false;
        private bool _wsummon = false;
        private int _option;
        private int _options;
        private int _columns;
        private readonly int _rows = 4;

        public BattleMenu(Ally a)
            : base(
                Seven.Config.WindowWidth / 4,
                Seven.Config.WindowHeight * 7 / 10 + 10,
                Seven.Config.WindowWidth / 5,
                Seven.Config.WindowHeight * 5 / 20 - 6)
        {

            Visible = false;


            // Always an option

            #region Item Menu

            foreach (MateriaBase m in a.Materia)
            {
                if (m != null && m.ID == "witem")
                {
                    _witem = true;
                }
            }

            #endregion Item Menu 




            // iterate through options
            int o = 0;

            #region Attack
            foreach (MateriaBase m in a.Materia)
            {
                if (m != null)
                {
                    if (m.ID == "doublecut")
                    {
                        if (m.Level == 0)
                        {
                            _doubleCutOption2 = o;
                            _doubleCutOption4 = -1;
                        }
                        else
                        {
                            _doubleCutOption4 = o;
                            _doubleCutOption2 = -1;
                        }
                        _slashAllOption = -1;
                        _flashOption = -1;
                    }
                    else if (m.ID == "slashall")
                    {
                        if (m.Level == 0)
                        {
                            _slashAllOption = o;
                            _flashOption = -1;
                        }
                        else
                        {
                            _flashOption = o;
                            _slashAllOption = -1;
                        }
                        _doubleCutOption2 = -1;
                        _doubleCutOption4 = -1;
                    }
                }
            }
            if (_doubleCutOption2 == -1 && _doubleCutOption4 == -1 &&
                _slashAllOption == -1 && _flashOption == -1)
            {
                _attackOption = o;
            }
            o++;
            #endregion Attack



            // put this after every one in order to skip the Item option
            if (o == 3)
            {
                o++;
            }



            #region Magic Menu
            int c = 0;
            foreach (MagicSpell s in a.MagicSpells)
            {
                c++;
            }
            if (c > 0)
            {
                _magicMenuOption = o;
                o++;
                foreach (MateriaBase m in a.Materia)
                {
                    if (m != null && m.ID == "wmagic")
                    {
                        _wmagic = true;
                    }
                }
            }
                    
            #endregion Magic Menu



            // put this after every one in order to skip the Item option
            if (o == 3)
            {
                o++;
            }



            #region Summon Menu
            c = 0;

            foreach (SummonSpell s in a.Summons)
            {
                c++;
            }

            if (c > 0)
            {
                _summonMenuOption = o;
                o++;
                foreach (MateriaBase m in a.Materia)
                {
                    if (m != null && m.ID == "wsummon")
                    {
                        _wsummon = true;
                    }
                }
            }
            #endregion Summon Menu



            // put this after every one in order to skip the Item option
            if (o == 3)
            {
                o++;
            }



            #region Sense

            foreach (MateriaBase m in a.Materia)
            {
                if (m != null && m.ID == "sense")
                {
                    _senseOption = o;
                    o++;
                }
            }

            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
            {
                o++;
            }



            #region Enemy Skill

            foreach (MateriaBase m in a.Materia)
            { 
                if (m != null && m.ID == "enemyskill")
                {
                    _enemySkillMenuOption = o;
                    o++;
                }
            }
            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
            {
                o++;
            }



            #region Mime

            foreach (MateriaBase m in a.Materia)
            {
                if (m != null && m.ID == "mime")
                {
                    _mimeOption = o;
                    o++;
                }
            }

            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
            {
                o++;
            }



            #region Deathblow

            foreach (MateriaBase m in a.Materia)
            {
                if (m != null && m.ID == "deathblow")
                {
                    _deathblowOption = o;
                    o++;
                }
            }
            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
            {
                o++;
            }



            #region Steal

            foreach (MateriaBase m in a.Materia)
            {
                if (m != null && m.ID == "steal")
                {
                    if (m.Level == 0)
                    {
                        _stealOption = o;
                        _mugOption = -1;
                    }
                    else
                    {
                        _mugOption = o;
                        _stealOption = -1;
                    }
                }
            }
            if (_mugOption != -1 || _stealOption != -1)
            {
                o++;
            }

            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
            {
                o++;
            }



            _columns = (o - 1) / _rows + 1;
            if (_columns != 1)
            {
                Width = W * _columns;
            }
            
        }

        public override void ControlHandle(Key k)
        {
            int col = _option / _rows;
            int row = _option % _rows;

            switch (k)
            {
                case Key.Up:
                    if (row > 0)
                    {
                        _option--;
                    }
                    break;
                case Key.Down:
                    //if (_option < _rows * _columns - 1) _option++;
                    if (row < _rows - 1)
                    {
                        _option++;
                    }
                    break;
                case Key.Right:
                    //if (_option + _rows < _rows * _columns - 1)
                    if (col < _columns - 1)
                    {
                        _option += _rows;
                    }
                    break;
                case Key.Left:
                    //if (_option - _rows >= 0)
                    if (col > 0)
                    {
                        _option -= _rows;
                    }
                    break;
                case Key.Circle:
                    if (_option == _attackOption)
                    {
                        Seven.BattleState.Screen.SelectCombatant(BattleTargetGroup.Enemies);
                    }
                    else if (_option == _doubleCutOption2 || _option == _doubleCutOption4)
                    {
                        //Seven.BattleState.Screen.GetSelection(TargetGroup.Area, TargetType.Combatant);
                    }
                    else if (_option == _slashAllOption)
                    {
                        //Seven.BattleState.Screen.GetSelection(TargetGroup.Area, TargetType.GroupNS);
                    }
                    else if (_option == _flashOption)
                    {
                        //Seven.BattleState.Screen.GetSelection(TargetGroup.Enemies, TargetType.GroupNS);
                    }
                    else if (_option == _magicMenuOption && !Seven.BattleState.Commanding.Silence)
                    {
                        Seven.BattleState.Screen.PushControl(Seven.BattleState.Commanding.MagicMenu);
                    }
                    else if (_option == _enemySkillMenuOption && !Seven.BattleState.Commanding.Silence)
                    {
                        //Seven.BattleState.Screen.PushControl(Seven.BattleState.Commanding.EnemySkillMenu);
                    }
                    else if (_option == _summonMenuOption && !Seven.BattleState.Commanding.Silence)
                    {
                        //Seven.BattleState.Screen.PushControl(Seven.BattleState.Commanding.SummonMenu);
                    }
                    else if (_option == _senseOption)
                    {
                        Seven.BattleState.Screen.SelectCombatant(BattleTargetGroup.Enemies);
                    }
                    else if (_option == _mimeOption)
                    {
                        //Seven.BattleState.Screen.GetSelection(TargetGroup.Area, TargetType.Self);
                    }
                    else if (_option == _deathblowOption)
                    {
                        //Seven.BattleState.Screen.GetSelection(TargetGroup.Area, TargetType.Combatant);
                    }
                    else if (_option == _stealOption || _option == _mugOption)
                    {
                        //Seven.BattleState.Screen.GetSelection(TargetGroup.Enemies, TargetType.Combatant);
                    }
                    else if (_option == _itemMenuOption)
                    {
                        if (_witem)
                        {
                            Seven.BattleState.Screen.PushControl(Seven.BattleState.Screen.WItemMenu);
                        }
                        else
                        {
                            Seven.BattleState.Screen.PushControl(Seven.BattleState.Screen.ItemMenu);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public bool ActOnSelection(IEnumerable<Combatant> targets)
        {
            Combatant target;
            Ally performer = Seven.BattleState.Commanding;

            #region Attack

            if (_option == _attackOption)
            {
                target = targets.First();

                performer.Attack(target);
            }

            #endregion Attack


            #region 2x-Cut

            else if (_option == _doubleCutOption2)
            {
                target = targets.First();
                
                performer.Attack(target, false);
                performer.Attack(target);
            }

            #endregion 2x-Cut

            #region 4x-Cut

            else if (_option == _doubleCutOption4)
            {
                target = targets.First();

                performer.Attack(target, false);
                performer.Attack(target, false);
                performer.Attack(target, false);
                performer.Attack(target);
            }

            #endregion 4x-Cut

            #region Sense

            else if (_option == _senseOption)
            {
                target = targets.First();

                BattleEvent e = new BattleEvent(performer, () => target.Sense());
                
                e.Dialogue = c => target.ToString();
                
                Seven.BattleState.EnqueueAction(e);
            }

            #endregion Sense

            #region Mime

            else if (_option == _mimeOption)
            {
//                if (Seven.BattleState.LastPartyAbility == null)
//                {
////                    DisableActionHook(true);
//                    Seven.BattleState.Commanding.TurnTimer.Reset();
//                }
//                else
//                {
//                    Seven.BattleState.Commanding.Ability = Seven.BattleState.LastPartyAbility;
//                    try
//                    {
//                        Seven.BattleState.Commanding.Ability.Performer = performer;
//                    }
//                    catch (Exception e)
//                    {
//                    }
//                }
            }

            #endregion Mime

            #region Deathblow

            else if (_option == _deathblowOption)
            {
                target = targets.First();

                int bd = Formula.PhysicalBase(Seven.BattleState.Commanding);
                int dam = Formula.PhysicalDamage(bd, 16, target) * 2;

//                AbilityState state = Seven.BattleState.Commanding.Ability;
//                state.LongRange = performer.LongRange;
//                state.QuadraMagic = false;
//                state.Type = AttackType.Physical;
//                state.Performer = performer;
//                state.Target = Seven.BattleState.Screen.TargetSelector.Selected;
//                state.Action += delegate() { target.AcceptDamage(Seven.BattleState.ActiveAbility.Performer, dam); };
            }

            #endregion Deathblow

            #region Steal
            else if (_option == _stealOption)
            {
                target = targets.First();

                BattleEvent e = new BattleEvent(performer, () => ((Enemy)target).StealItem(performer));
                
                e.Dialogue = c => target.ToString() + " being theived";
                
                Seven.BattleState.EnqueueAction(e);
            }
            #endregion Steal
            #region Mug
            else if (_option == _mugOption)
            {
                target = targets.First();

                int bd = Formula.PhysicalBase(Seven.BattleState.Commanding);
                int dam = Formula.PhysicalDamage(bd, 16, target);

//                AbilityState state = Seven.BattleState.Commanding.Ability;
//                state.Type = AttackType.Physical;
//                state.Performer = performer;
//                state.Target = Seven.BattleState.Screen.TargetSelector.Selected;
//                state.Action += delegate() 
//                { 
//                    target.AcceptDamage(Seven.BattleState.ActiveAbility.Performer, dam);
//                    ((Enemy)target).StealItem(performer);
//                };
            }
            #endregion Mug

            return true;
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Color white = new Color(1, 1, 1);
            Color gray = new Color(.4, .4, .4);
            Color c = white;



            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx + (_option / _rows * xs), Y + cy + (_option % _rows * y));
            }



            // "Attack"
            if (_slashAllOption != -1)
            {
                Text.ShadowedText(g, c, "Slash-All", X + x, Y + ((_slashAllOption % _rows + 1) * y) + y0);
            }
            else if (_flashOption != -1)
            {
                Text.ShadowedText(g, c, "Flash", X + x, Y + ((_flashOption % _rows + 1) * y) + y0);
            }
            else if (_doubleCutOption2 != -1)
            {
                Text.ShadowedText(g, c, "2x-Cut", X + x, Y + ((_doubleCutOption2 % _rows + 1) * y) + y0);
            }
            else if (_doubleCutOption4 != -1)
            {
                Text.ShadowedText(g, c, "4x-Cut", X + x, Y + ((_doubleCutOption4 % _rows + 1) * y) + y0);
            }
            else
            {
                Text.ShadowedText(g, c, "Attack", X + x, Y + ((_attackOption % _rows + 1) * y) + y0);
            }

            

            // Magic
            if (_magicMenuOption != -1)
            {
                if (Seven.BattleState.Commanding.Silence || Seven.BattleState.Commanding.Frog)
                {
                    c = gray;
                }
                else
                {
                    c = white;
                }
                Text.ShadowedText(g, c, _wmagic ? "W-Magic" : "Magic", 
                    X + x + xs * (_magicMenuOption / _rows), 
                    Y + ((_magicMenuOption % _rows + 1) * y) + y0);
            }

            // Summon
            if (_summonMenuOption != -1)
            {
                if (Seven.BattleState.Commanding.Silence || Seven.BattleState.Commanding.Frog)
                {
                    c = gray;
                }
                else
                {
                    c = white;
                }
                Text.ShadowedText(g, c, _wsummon ? "W-Summon" : "Summon", 
                    X + x + xs * (_summonMenuOption / _rows),
                    Y + ((_summonMenuOption % _rows + 1) * y) + y0);
            }

            // Sense
            if (_senseOption != -1)
            {
                c = white;
                Text.ShadowedText(g, c, "Sense",
                    X + x + xs * (_senseOption / _rows),
                    Y + ((_senseOption % _rows + 1) * y) + y0);
            }

            // Enemy Skill
            if (_enemySkillMenuOption != -1)
            {
                if (Seven.BattleState.Commanding.Silence || Seven.BattleState.Commanding.Frog)
                {
                    c = gray;
                }
                else
                {
                    c = white;
                }
                Text.ShadowedText(g, c, "E.Skill",
                    X + x + xs * (_enemySkillMenuOption / _rows),
                    Y + ((_enemySkillMenuOption % _rows + 1) * y) + y0);
            }

            // Mime
            if (_mimeOption != -1)
            {
                c = white;
                Text.ShadowedText(g, c, "Mime",
                    X + x + xs * (_mimeOption / _rows),
                    Y + ((_mimeOption % _rows + 1) * y) + y0);
            }

            // D.Blow
            if (_deathblowOption != -1)
            {
                c = white;
                Text.ShadowedText(g, c, "D.Blow",
                    X + x + xs * (_deathblowOption / _rows),
                    Y + ((_deathblowOption % _rows + 1) * y) + y0);
            }

            // Steal/Mug
            if (_stealOption != -1)
            {
                c = white;
                Text.ShadowedText(g, c, "Steal",
                    X + x + xs * (_stealOption / _rows),
                    Y + ((_stealOption % _rows + 1) * y) + y0);
            }
            else if (_mugOption != -1)
            {
                c = white;
                Text.ShadowedText(g, c, "Mug",
                    X + x + xs * (_mugOption / _rows),
                    Y + ((_mugOption % _rows + 1) * y) + y0);
            }

            // Item
            if (_itemMenuOption != -1)
            {
                c = white;
                Text.ShadowedText(g, c, _witem ? "W-Item" : "Item", 
                    X + x + xs * (_itemMenuOption / _rows),
                    Y + ((_itemMenuOption % _rows + 1) * y) + y0);
            }




            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
        }

        public override void Reset()
        {
            _option = 0;
            Visible = false;
        }

        public override string Info
        {
            get
            {
                switch (_option)
                {
                    case 0:
                        return "Attack an enemy";
                    case 3:
                        return "Use an item";
                    default:
                        return "";
                }
            }
        }

        public bool WItem { get { return _witem; } }

        public bool WMagic { get { return _wmagic; } }

        public bool WSummon { get { return _wsummon; } }
    }


}