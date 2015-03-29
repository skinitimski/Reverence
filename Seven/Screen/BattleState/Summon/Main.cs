using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;
using SummonSpell = Atmosphere.Reverence.Seven.Asset.Summon;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Summon
{
    internal class Main : ControlMenu, ISelectorUser
    {
        private int COLUMNS = 1;

        #region Layout

        const int x1 = 200;
        const int ys = 35;
        const int cx = -15;
        const int cy = -8;

        #endregion Layout

        private int _option;
        private int _topRow;
        private int _totalRows = SummonSpell.TOTAL_SUMMONS;
        private readonly int _visibleRows = 3;
        private bool _wsummon;
        private int _first = -1;
        private SummonSpell[] _summons;

        public Main(IEnumerable<SummonSpell> summons, bool wsummon)
            : base(
                5,
                Config.Instance.WindowHeight * 7 / 10 + 20,
                Config.Instance.WindowWidth * 3 / 4,
                (Config.Instance.WindowHeight * 5 / 20) - 25)
        {
            _wsummon = wsummon;

            _summons = new SummonSpell[_totalRows];

            foreach (SummonSpell s in summons)
                _summons[s.Order] = s;

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
                        if (Seven.BattleState.Commanding.MP >= _summons[_option].Spell.MPCost)
                        {
                            if (_first == -1 && _wsummon)
                            {
                                Seven.BattleState.Screen.DisableActionHook();
                            }

                            Spell spell = _summons[_option].Spell;

                            Seven.BattleState.Screen.GetSelection(spell.Target, spell.TargetEnemiesFirst);
                        }
                        else
                        {
                            // TODO: beep
                        }
                    }
                    break;
            }
        }

        public void ActOnSelection()
        {
            if (_first != -1 || !_wsummon)
            {
                Seven.BattleState.Commanding.UseMP(_summons[_option].Spell.MPCost);

                #region First

//                AbilityState state;
//
//                if (_wsummon)
//                {
//                    state = (AbilityState)Seven.BattleState.Commanding.Ability.Clone();
//
//                    state.Target = Seven.BattleState.Screen.TargetSelector.Selected;
//                    state.Performer = Seven.BattleState.Commanding;
//
//                    state.Action += delegate() { _summons[_option].Spell.Action(); };
//
//                    Seven.BattleState.EnqueueAction(state);
//                }

                #endregion First

                #region Second

//                state = Seven.BattleState.Commanding.Ability;
//
//                state.Target = Seven.BattleState.Screen.TargetSelector.Selected;
//                state.Performer = Seven.BattleState.Commanding;
//
//                state.Action += delegate() { _summons[_option].Spell.Action(); };

                // don't enqueue, will be picked up by Battle.ActionHook()

                #endregion Second
            }
            else
            {
                _first = _option;
            }
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
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
            _first = -1;
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
                foreach (SummonSpell s in _summons)
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

        public SummonSpell Selected { get { return _summons[_option]; } }
    }





}
