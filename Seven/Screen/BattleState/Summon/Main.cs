using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

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
        private int _totalRows = Summon.TOTAL_SUMMONS;
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
                    if (_option > 0) _option--;
                    if (_topRow > _option) _topRow--;
                    break;
                case Key.Down:
                    if (_option < _totalRows - 1) _option++;
                    if (_topRow < _option - _visibleRows + 1) _topRow++;
                    break;
                case Key.X:
                    Visible = false;
                    BattleScreen.Instance.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    if (!String.IsNullOrEmpty(_summons[_option].ID))
                        if (Seven.BattleState.Commanding.MP >= _summons[_option].Spell.Cost)
                        {
                            if (_first == -1 && _wsummon)
                                Selector.DisableActionHook();
                            _summons[_option].Spell.Dispatch();
                        }
                    break;
            }
        }
        public void ActOnSelection()
        {
            if (_first != -1 || !_wsummon)
            {
                Seven.BattleState.Commanding.MP -= _summons[_option].Spell.Cost;

                #region First

                AbilityState state;

                if (_wsummon)
                {
                    state = (AbilityState)Seven.BattleState.Commanding.Ability.Clone();

                    state.Target = TargetSelector.Instance.Selected;
                    state.Performer = Seven.BattleState.Commanding;

                    state.Action += delegate() { _summons[_option].Spell.Action(); };

                    Seven.BattleState.EnqueueAction(state);
                }

                #endregion First

                #region Second

                state = Seven.BattleState.Commanding.Ability;

                state.Target = TargetSelector.Instance.Selected;
                state.Performer = Seven.BattleState.Commanding;

                state.Action += delegate() { _summons[_option].Spell.Action(); };

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

            TextExtents te;


            int j = Math.Min(_visibleRows + _topRow, _totalRows);

            for (int b = _topRow; b < j; b++)
                Text.ShadowedText(g, String.IsNullOrEmpty(_summons[b].ID) ? "" : _summons[b].Name,
                    X + x1,
                    Y + (b - _topRow + 1) * ys);


            if (IsControl)
                Shapes.RenderCursor(g,
                    X + x1 + cx,
                    Y + cy + (_option - _topRow + 1) * ys);

            SummonMenuInfo.Instance.Draw(d);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void Reset()
        {
            _option = 0;
            _topRow = 0;
            Visible = false;
            SummonMenuInfo.Instance.Visible = false;
            _first = -1;
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
            SummonMenuInfo.Instance.Visible = true;
        }

        public bool IsValid
        {
            get
            {
                foreach (Summon s in _summons)
                    if (!String.IsNullOrEmpty(_summons[_option].ID))
                        return true;
                return false;
            }
        }        

        public override string Info { get { return String.IsNullOrEmpty(_summons[_option].ID) ? "" : _summons[_option].Spell.Desc; } }

        public new SummonSpell Selected { get { return _summons[_option]; } }
    }





}
