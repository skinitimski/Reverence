using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.BattleSimulator
{

    public class SummonMenu : ControlMenu, ISelectorUser
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

        private Summon[] _summons;


        public SummonMenu(IEnumerable<Summon> summons, bool wsummon)
            : base(
                5,
                Globals.HEIGHT * 7 / 10 + 20,
                Globals.WIDTH * 3 / 4,
                (Globals.HEIGHT * 5 / 20) - 25)
        {
            _wsummon = wsummon;

            _summons = new Summon[_totalRows];

            foreach (Summon s in summons)
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
                        if (Game.Battle.Commanding.MP >= _summons[_option].Spell.Cost)
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
                Game.Battle.Commanding.MP -= _summons[_option].Spell.Cost;

                #region First

                AbilityState state;

                if (_wsummon)
                {
                    state = (AbilityState)Game.Battle.Commanding.Ability.Clone();

                    state.Target = TargetSelector.Instance.Selected;
                    state.Performer = Game.Battle.Commanding;

                    state.Action += delegate() { _summons[_option].Spell.Action(); };

                    Game.Battle.EnqueueAction(state);
                }

                #endregion First

                #region Second

                state = Game.Battle.Commanding.Ability;

                state.Target = TargetSelector.Instance.Selected;
                state.Performer = Game.Battle.Commanding;

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
                Graphics.ShadowedText(g, String.IsNullOrEmpty(_summons[b].ID) ? "" : _summons[b].Name,
                    X + x1,
                    Y + (b - _topRow + 1) * ys);


            if (IsControl)
                Graphics.RenderCursor(g,
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
        public override string Info
        { get { return String.IsNullOrEmpty(_summons[_option].ID) ? "" : _summons[_option].Spell.Desc; } }
        public new Summon Selected { get { return _summons[_option]; } }
    }




    internal sealed class SummonMenuInfo : Menu
    {
        public static SummonMenuInfo Instance;

        #region Layout

        const int x0 = 15; // ability col
        const int x1 = 40; // mpneeded col
        const int x2 = 90; // slash col
        const int x3 = 130; // mptot col
        const int y0 = 30; // 
        const int ys = 30;

        #endregion Layout

        static SummonMenuInfo()
        {
            Instance = new SummonMenuInfo();
        }
        private SummonMenuInfo()
            : base(
                Globals.WIDTH * 3 / 4 + 12,
                Globals.HEIGHT * 7 / 10 + 20,
                Globals.WIDTH / 4 - 17,
                (Globals.HEIGHT * 5 / 20) - 25)
        {
            Visible = false;
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            Summon s = Game.Battle.Commanding.SummonMenu.Selected;

            int row = 0;

            if (!String.IsNullOrEmpty(s.ID))
            {
                string cost = s.Spell.Cost.ToString();
                Graphics.ShadowedText(g, "MP Req", X + x1, Y + y0);

                row++;

                cost = cost + "/";
                te = g.TextExtents(cost);
                Graphics.ShadowedText(g, cost, X + x2 - te.Width, Y + y0 + (row * ys));

                string tot = Game.Battle.Commanding.MP.ToString();
                te = g.TextExtents(tot);
                Graphics.ShadowedText(g, tot, X + x3 - te.Width, Y + y0 + (row * ys));

                row++;

                if (s.AddedAbility.Contains(AddedAbility.All))
                {
                    string msg = "All x";
                    msg += s.AllCount.ToString();
                    Graphics.ShadowedText(g, msg, X + x0, Y + y0 + (row * ys));
                    row++;
                }

                if (s.AddedAbility.Contains(AddedAbility.QuadraMagic))
                {
                    string msg = "Q-Magic x";
                    msg += s.QMagicCount.ToString();
                    Graphics.ShadowedText(g, msg, X + x0, Y + y0 + (row * ys));
                    row++;
                }

            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }


}
