using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Summon
{
    internal class Main : ControlMenu, ISelectorUser
    {
        #region Layout

        const int x1 = 200;
        const int ys = 35;
        const int cx = -15;
        const int cy = -8;

        #endregion Layout

        protected int _option;
        private int _topRow;
        private int _totalRows = SummonSpell.Count;
        private readonly int _visibleRows = 3;
        protected SummonMenuEntry[] _summons;

        public Main(IEnumerable<SummonMenuEntry> summons, Menu.ScreenState screenState)
            : base(
                5,
                screenState.Height * 7 / 10 + 20,
                screenState.Width * 3 / 4,
                (screenState.Height * 5 / 20) - 25)
        {
            _summons = new SummonMenuEntry[_totalRows];

            foreach (SummonMenuEntry s in summons)
            {
                _summons[s.Order] = s;
            }

            Reset();
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (_option > 0)
                    {
                        _option--;
                    }
                    if (_topRow > _option)
                    {
                        _topRow--;
                    }
                    break;
                case Key.Down:
                    if (_option < _totalRows - 1)
                    {
                        _option++;
                    }
                    if (_topRow < _option - _visibleRows + 1)
                    {
                        _topRow++;
                    }
                    break;
                case Key.X:
                    Visible = false;
                    Seven.BattleState.Screen.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    if (_summons[_option] != null)
                    {
                        Spell spell = Selected.Spell;

                        if (CommandingAvailableMP >= spell.MPCost)
                        {
                            Seven.BattleState.Screen.ActivateSelector(spell.Target, spell.TargetEnemiesFirst);
                        }
                        else
                        {
                            Seven.Instance.ShowMessage(c => "!", 500);
                        }
                    }
                    break;
            }
        }
        
        public virtual bool ActOnSelection(IEnumerable<Combatant> targets)
        {
            UseSpell(_option, targets);
            
            return true;
        }
        
        protected void UseSpell(int option, IEnumerable<Combatant> targets, bool releaseAlly = true)
        {
            Ability spell = _summons[option].Spell;
            
            spell.Use(Seven.BattleState.Commanding, targets, new AbilityModifiers { ResetTurnTimer = releaseAlly});
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);


            int j = Math.Min(_visibleRows + _topRow, _totalRows);

            for (int b = _topRow; b < j; b++)
            {
                Text.ShadowedText(g, _summons[b] == null ? "" : _summons[b].Name,
                    X + x1,
                    Y + (b - _topRow + 1) * ys);
            }

            if (IsControl)
            {
                Shapes.RenderCursor(g,
                    X + x1 + cx,
                    Y + cy + (_option - _topRow + 1) * ys);
            }

            Seven.BattleState.Screen.SummonMenuInfo.Draw(d);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void Reset()
        {
            _option = 0;
            _topRow = 0;
            Visible = false;
            Seven.BattleState.Screen.SummonMenuInfo.Visible = false;
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
            Seven.BattleState.Screen.SummonMenuInfo.Visible = true;
        }

        public bool IsValid
        {
            get
            {
                foreach (SummonMenuEntry s in _summons)
                {
                    if (s != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override string Info { get { return _summons[_option] == null ? "" : _summons[_option].Spell.Desc; } }

        public SummonMenuEntry Selected { get { return _summons[_option]; } }
        
        protected virtual int CommandingAvailableMP { get { return Seven.BattleState.Commanding.MP; } }
    }





}
