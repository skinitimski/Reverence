using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    public class BattleMenu : ControlMenu, ISelectorUser
    {
        #region Layout

        const int x = 35;
        const int y = 30;
        const int y0 = 5;
        const int cx = 20;
        const int cy = 28;
        const int xs = Globals.WIDTH / 5; // W

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
                Globals.WIDTH / 4,
                Globals.HEIGHT * 7 / 10 + 10,
                Globals.WIDTH / 5,
                Globals.HEIGHT * 5 / 20 - 6)
        {

            Visible = false;


            // Always an option

            #region Item Menu

            foreach (Materia m in a.Materia)
                if (m != null && m.Name == "W-Item")
                    _witem = true;

            #endregion Item Menu 




            // iterate through options
            int o = 0;

            #region Attack
            foreach (Materia m in a.Materia)
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
            if (_doubleCutOption2 == -1 && _doubleCutOption4 == -1 &&
                _slashAllOption == -1 && _flashOption == -1)
                _attackOption = o;
            o++;
            #endregion Attack



            // put this after every one in order to skip the Item option
            if (o == 3)
                o++;



            #region Magic Menu
            int c = 0;
            foreach (MagicSpell s in a.MagicSpells)
                c++;
            if (c > 0)
            {
                _magicMenuOption = o;
                o++;
                foreach (Materia m in a.Materia)
                    if (m != null && m.Name == "W-Magic")
                        _wmagic = true;
            }
            #endregion Magic Menu



            // put this after every one in order to skip the Item option
            if (o == 3)
                o++;



            #region Summon Menu
            c = 0;
            foreach (Summon s in a.Summons)
                c++;
            if (c > 0)
            {
                _summonMenuOption = o;
                o++;
                foreach (Materia m in a.Materia)
                    if (m != null && m.Name == "W-Summon")
                        _wsummon = true;
            }
            #endregion Summon Menu



            // put this after every one in order to skip the Item option
            if (o == 3)
                o++;



            #region Sense

            foreach (Materia m in a.Materia)
                if (m != null && m.Name == "Sense")
                {
                    _senseOption = o;
                    o++;
                }

            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
                o++;



            #region Enemy Skill

            foreach (Materia m in a.Materia)
                if (m != null && m.ID == "enemyskill")
                {
                    _enemySkillMenuOption = o;
                    o++;
                }

            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
                o++;



            #region Mime

            foreach (Materia m in a.Materia)
                if (m != null && m.ID == "mime")
                {
                    _mimeOption = o;
                    o++;
                }

            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
                o++;



            #region Deathblow

            foreach (Materia m in a.Materia)
                if (m != null && m.ID == "deathblow")
                {
                    _deathblowOption = o;
                    o++;
                }

            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
                o++;



            #region Steal

            foreach (Materia m in a.Materia)
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
            if (_mugOption != -1 || _stealOption != -1)
                o++;

            #endregion



            // put this after every one in order to skip the Item option
            if (o == 3)
                o++;



            _columns = (o - 1) / _rows + 1;
            if (_columns != 1)
                Width = W * _columns;
            
        }

        public override void ControlHandle(Key k)
        {
            int col = _option / _rows;
            int row = _option % _rows;

            switch (k)
            {
                case Key.Up:
                    if (row > 0) _option--;
                    break;
                case Key.Down:
                    //if (_option < _rows * _columns - 1) _option++;
                    if (row < _rows - 1) _option++;
                    break;
                case Key.Right:
                    //if (_option + _rows < _rows * _columns - 1)
                    if (col < _columns - 1)
                        _option += _rows;
                    break;
                case Key.Left:
                    //if (_option - _rows >= 0)
                    if (col > 0)
                        _option -= _rows;
                    break;
                case Key.Circle:
                    if (_option == _attackOption) 
                        Game.Battle.Screen.GetSelection(TargetGroup.Area, TargetType.OneTar);

                    else if (_option == _doubleCutOption2 || _option == _doubleCutOption4)
                        Game.Battle.Screen.GetSelection(TargetGroup.Area, TargetType.OneTar);

                    else if (_option == _slashAllOption) 
                        Game.Battle.Screen.GetSelection(TargetGroup.Area, TargetType.AllTarNS);

                    else if (_option == _flashOption)
                        Game.Battle.Screen.GetSelection(TargetGroup.Enemies, TargetType.AllTarNS);

                    else if (_option == _magicMenuOption && !Game.Battle.Commanding.Silence)
                        Game.Battle.Screen.PushControl(Game.Battle.Commanding.MagicMenu);

                    else if (_option == _enemySkillMenuOption && !Game.Battle.Commanding.Silence)
                        Game.Battle.Screen.PushControl(Game.Battle.Commanding.EnemySkillMenu);

                    else if (_option == _summonMenuOption && !Game.Battle.Commanding.Silence)
                        Game.Battle.Screen.PushControl(Game.Battle.Commanding.SummonMenu);

                    else if (_option == _senseOption)
                        Game.Battle.Screen.GetSelection(TargetGroup.Area, TargetType.OneTar);

                    else if (_option == _mimeOption)
                        Game.Battle.Screen.GetSelection(TargetGroup.Area, TargetType.Self);

                    else if (_option == _deathblowOption)
                        Game.Battle.Screen.GetSelection(TargetGroup.Area, TargetType.OneTar);

                    else if (_option == _stealOption || _option == _mugOption)
                        Game.Battle.Screen.GetSelection(TargetGroup.Enemies, TargetType.OneTar);

                    else if (_option == _itemMenuOption)
                        if (_witem)
                            Game.Battle.Screen.PushControl(WItemMenu.Instance);
                        else
                            Game.Battle.Screen.PushControl(ItemMenu.Instance);
                    break;
                default:
                    break;
            }
        }

        public void ActOnSelection()
        {
            ICombatant target;
            Ally performer = Game.Battle.Commanding;

            #region Attack
            if (_option == _attackOption)
            {
                target = TargetSelector.Instance.Selected[0];

                int bd = Formula.PhysicalBase(Game.Battle.Commanding);
                int dam = Formula.PhysicalDamage(bd, 16, target);

                AbilityState state = Game.Battle.Commanding.Ability;
                state.LongRange = performer.LongRange;
                state.QuadraMagic = false;
                state.Type = AttackType.Physical;
                state.Performer = performer;
                state.Target = TargetSelector.Instance.Selected;
                state.Action += delegate() { target.AcceptDamage(Game.Battle.ActiveAbility.Performer, dam); };
            }
            #endregion Attack
            #region 2x-Cut
            else if (_option == _doubleCutOption2)
            {
                target = TargetSelector.Instance.Selected[0];

                int bd = Formula.PhysicalBase(Game.Battle.Commanding);
                int dam = Formula.PhysicalDamage(bd, 16, target);

                AbilityState state = Game.Battle.Commanding.Ability;
                state.LongRange = performer.LongRange;
                state.QuadraMagic = false;
                state.Type = AttackType.Physical;
                state.Performer = performer;
                state.Target = TargetSelector.Instance.Selected;
                state.Action += delegate() { target.AcceptDamage(Game.Battle.ActiveAbility.Performer, dam); };

                Game.Battle.EnqueueAction((AbilityState)state.Clone());
                Game.Battle.EnqueueAction((AbilityState)state.Clone());

                // Here we must disable the action hook, because we do not the second state
                //  to be attached to Ally.Ability. If it were, when the first one is disposed of
                //  it will reset the second one. See BattleState.CheckAbilityQueue()
                Selector.DisableActionHook(true);
            }
            #endregion 2x-Cut
            #region 4x-Cut
            else if (_option == _doubleCutOption4)
            {
                target = TargetSelector.Instance.Selected[0];

                int bd = Formula.PhysicalBase(Game.Battle.Commanding);
                int dam = Formula.PhysicalDamage(bd, 16, target);

                AbilityState state = Game.Battle.Commanding.Ability;
                state.LongRange = performer.LongRange;
                state.QuadraMagic = false;
                state.Type = AttackType.Physical;
                state.Performer = performer;
                state.Target = TargetSelector.Instance.Selected;
                state.Action += delegate() { target.AcceptDamage(Game.Battle.ActiveAbility.Performer, dam); };

                Game.Battle.EnqueueAction((AbilityState)state.Clone());
                Game.Battle.EnqueueAction((AbilityState)state.Clone());
                Game.Battle.EnqueueAction((AbilityState)state.Clone());
                Game.Battle.EnqueueAction((AbilityState)state.Clone());

                // Here we must disable the action hook, because we do not the first three states
                //  to be attached to Ally.Ability. If it were, when the first one is disposed of
                //  it will reset the others. See BattleState.CheckAbilityQueue()
                Selector.DisableActionHook(true);
            }
            #endregion 4x-Cut
            #region Sense
            else if (_option == _senseOption)
            {
                target = TargetSelector.Instance.Selected[0];

                AbilityState state = Game.Battle.Commanding.Ability;
                state.Performer = performer;
                state.Target = new ICombatant[1];
                state.Target[0] = target;
                state.Action += delegate() { target.Sense(); };
            }
            #endregion Sense
            #region Mime
            else if (_option == _mimeOption)
            {
                if (Game.Battle.LastPartyAbility == null)
                {
                    Selector.DisableActionHook(true);
                    Game.Battle.Commanding.TurnTimer.Reset();
                }
                else
                {
                    Game.Battle.Commanding.Ability = Game.Battle.LastPartyAbility;
                    try
                    {
                        Game.Battle.Commanding.Ability.Performer = performer;
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            #endregion Mime
            #region Deathblow
            else if (_option == _deathblowOption)
            {
                target = TargetSelector.Instance.Selected[0];

                int bd = Formula.PhysicalBase(Game.Battle.Commanding);
                int dam = Formula.PhysicalDamage(bd, 16, target) * 2;

                AbilityState state = Game.Battle.Commanding.Ability;
                state.LongRange = performer.LongRange;
                state.QuadraMagic = false;
                state.Type = AttackType.Physical;
                state.Performer = performer;
                state.Target = TargetSelector.Instance.Selected;
                state.Action += delegate() { target.AcceptDamage(Game.Battle.ActiveAbility.Performer, dam); };
            }
            #endregion Deathblow
            #region Steal
            else if (_option == _stealOption)
            {
                target = TargetSelector.Instance.Selected[0];

                AbilityState state = Game.Battle.Commanding.Ability;
                state.Type = AttackType.Physical;
                state.Performer = performer;
                state.Target = TargetSelector.Instance.Selected;
                state.Action += delegate() { ((Enemy)target).StealItem(performer); };
            }
            #endregion Steal
            #region Mug
            else if (_option == _mugOption)
            {
                target = TargetSelector.Instance.Selected[0];

                int bd = Formula.PhysicalBase(Game.Battle.Commanding);
                int dam = Formula.PhysicalDamage(bd, 16, target);

                AbilityState state = Game.Battle.Commanding.Ability;
                state.Type = AttackType.Physical;
                state.Performer = performer;
                state.Target = TargetSelector.Instance.Selected;
                state.Action += delegate() 
                { 
                    target.AcceptDamage(Game.Battle.ActiveAbility.Performer, dam);
                    ((Enemy)target).StealItem(performer);
                };
            }
            #endregion Mug

        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Color white = new Color(1, 1, 1);
            Color gray = new Color(.4, .4, .4);
            Color c = white;



            if (IsControl)
                Graphics.RenderCursor(g, X + cx + (_option / _rows * xs), Y + cy + (_option % _rows * y));



            // "Attack"
            if (_slashAllOption != -1)
                Graphics.ShadowedText(g, c, "Slash-All", X + x, Y + ((_slashAllOption % _rows + 1) * y) + y0);
            else if (_flashOption != -1)
                Graphics.ShadowedText(g, c, "Flash", X + x, Y + ((_flashOption % _rows + 1) * y) + y0);
            else if (_doubleCutOption2 != -1)
                Graphics.ShadowedText(g, c, "2x-Cut", X + x, Y + ((_doubleCutOption2 % _rows + 1) * y) + y0);
            else if (_doubleCutOption4 != -1)
                Graphics.ShadowedText(g, c, "4x-Cut", X + x, Y + ((_doubleCutOption4 % _rows + 1) * y) + y0);
            else
                Graphics.ShadowedText(g, c, "Attack", X + x, Y + ((_attackOption % _rows + 1) * y) + y0);

            

            // Magic
            if (_magicMenuOption != -1)
            {
                if (Game.Battle.Commanding.Silence || Game.Battle.Commanding.Frog)
                    c = gray;
                else
                    c = white;
                Graphics.ShadowedText(g, c, _wmagic ? "W-Magic" : "Magic", 
                    X + x + xs * (_magicMenuOption / _rows), 
                    Y + ((_magicMenuOption % _rows + 1) * y) + y0);
            }

            // Summon
            if (_summonMenuOption != -1)
            {
                if (Game.Battle.Commanding.Silence || Game.Battle.Commanding.Frog)
                    c = gray;
                else
                    c = white;
                Graphics.ShadowedText(g, c, _wsummon ? "W-Summon" : "Summon", 
                    X + x + xs * (_summonMenuOption / _rows),
                    Y + ((_summonMenuOption % _rows + 1) * y) + y0);
            }

            // Sense
            if (_senseOption != -1)
            {
                c = white;
                Graphics.ShadowedText(g, c, "Sense",
                    X + x + xs * (_senseOption / _rows),
                    Y + ((_senseOption % _rows + 1) * y) + y0);
            }

            // Enemy Skill
            if (_enemySkillMenuOption != -1)
            {
                if (Game.Battle.Commanding.Silence || Game.Battle.Commanding.Frog)
                    c = gray;
                else
                    c = white;
                Graphics.ShadowedText(g, c, "E.Skill",
                    X + x + xs * (_enemySkillMenuOption / _rows),
                    Y + ((_enemySkillMenuOption % _rows + 1) * y) + y0);
            }

            // Mime
            if (_mimeOption != -1)
            {
                c = white;
                Graphics.ShadowedText(g, c, "Mime",
                    X + x + xs * (_mimeOption / _rows),
                    Y + ((_mimeOption % _rows + 1) * y) + y0);
            }

            // D.Blow
            if (_deathblowOption != -1)
            {
                c = white;
                Graphics.ShadowedText(g, c, "D.Blow",
                    X + x + xs * (_deathblowOption / _rows),
                    Y + ((_deathblowOption % _rows + 1) * y) + y0);
            }

            // Steal/Mug
            if (_stealOption != -1)
            {
                c = white;
                Graphics.ShadowedText(g, c, "Steal",
                    X + x + xs * (_stealOption / _rows),
                    Y + ((_stealOption % _rows + 1) * y) + y0);
            }
            else if (_mugOption != -1)
            {
                c = white;
                Graphics.ShadowedText(g, c, "Mug",
                    X + x + xs * (_mugOption / _rows),
                    Y + ((_mugOption % _rows + 1) * y) + y0);
            }

            // Item
            if (_itemMenuOption != -1)
            {
                c = white;
                Graphics.ShadowedText(g, c, _witem ? "W-Item" : "Item", 
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