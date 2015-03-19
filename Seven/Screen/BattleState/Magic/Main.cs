using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Magic
{
       
    internal class Main : ControlMenu, ISelectorUser
    {
        private const int COLUMNS = 3;

        #region Layout

        const int x1 = 50;
        const int xs = 200;
        const int ys = 35;
        const int cx = -15;
        const int cy = -8;

        #endregion Layout

        private int _xopt;
        private int _yopt;
        private int _topRow;
        private int _totalRows = (MagicSpell.TOTAL_SPELLS / COLUMNS) + ((MagicSpell.TOTAL_SPELLS % COLUMNS == 0) ? 0 : 1);
        private readonly int _visibleRows = 3;

        private bool _wmagic;
        private int[] _first = new int[] { -1, -1 };


        private MagicSpell[,] _spells;

        public Main(IEnumerable<MagicSpell> spells, bool wmagic)
            : base(
                5,
                Config.Instance.WindowHeight * 7 / 10 + 20,
                Config.Instance.WindowWidth * 3 / 4,
                (Config.Instance.WindowHeight * 5 / 20) - 25)
        {
            _wmagic = wmagic;

            _spells = new MagicSpell[_totalRows,COLUMNS];

            foreach (MagicSpell s in spells)
                _spells[s.Spell.Order / COLUMNS, s.Spell.Order % COLUMNS] = s;

            Squish();

            Reset();
        }

        private void Squish()
        {
            int a, b, r;

            List<int> rowsToUse = new List<int>();

            for (b = 0; b < _totalRows; b++)
                for (a = 0; a < COLUMNS; a++)
                    if (!String.IsNullOrEmpty(_spells[b, a].ID)) { rowsToUse.Add(b); break; }

            MagicSpell[,] newSpells = new MagicSpell[rowsToUse.Count, COLUMNS];

            r = 0;
            foreach (int i in rowsToUse)
            {
                for (int j = 0; j < COLUMNS; j++)
                    newSpells[r, j] = _spells[i, j];
                r++;
            }

            _spells = newSpells;

            _totalRows = rowsToUse.Count;
        }




        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (_yopt > 0) _yopt--;
                    if (_topRow > _yopt) _topRow--;
                    break;
                case Key.Down:
                    if (_yopt < _totalRows - 1) _yopt++;
                    if (_topRow < _yopt - _visibleRows + 1) _topRow++;
                    break;
                case Key.Left:
                    if (_xopt > 0) _xopt--;
                    break;
                case Key.Right:
                    if (_xopt < COLUMNS - 1) _xopt++;
                    break;
                case Key.X:
                    Visible = false;
                    BattleScreen.Instance.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    if (!String.IsNullOrEmpty(_spells[_yopt, _xopt].ID))
                        if (_wmagic)
                        {
                            if (_first[0] == -1)
                            {
                                Selector.DisableActionHook();
                                if (Seven.BattleState.Commanding.MP >= _spells[_yopt, _xopt].Spell.Cost)
                                    _spells[_yopt, _xopt].Spell.Dispatch();
                            }
                            else
                            {
                                int mpAfterFirst = Seven.BattleState.Commanding.MP - _spells[_first[0], _first[1]].Spell.Cost;
                                if (mpAfterFirst >= _spells[_yopt, _xopt].Spell.Cost)
                                    _spells[_yopt, _xopt].Spell.Dispatch();
                            }
                        }
                        else
                        {
                            if (Seven.BattleState.Commanding.MP >= _spells[_yopt, _xopt].Spell.Cost)
                                _spells[_yopt, _xopt].Spell.Dispatch();
                        }
                    break;
            }
        }
        public void ActOnSelection()
        {

            if (_first[0] != -1 || !_wmagic)
            {
                AbilityState state;
                MagicSpell spell;

                if (_wmagic)
                {
                    Seven.BattleState.Commanding.MP -= _spells[_first[0], _first[1]].Spell.Cost;
                    Seven.BattleState.Commanding.MP -= _spells[_yopt, _xopt].Spell.Cost;

                    #region First

                    state = (AbilityState)Seven.BattleState.Commanding.Ability.Clone();
                    spell = _spells[_first[0], _first[1]];

                    state.Performer = Seven.BattleState.Commanding;
                    state.Action += delegate() { spell.Spell.Action(); };

                    Seven.BattleState.EnqueueAction(state);

                    #endregion First


                    #region Second

                    state = (AbilityState)Seven.BattleState.Commanding.Ability.Clone();
                    spell = _spells[_xopt, _yopt];

                    state.Performer = Seven.BattleState.Commanding;
                    state.Action += delegate() { spell.Spell.Action(); };

                    Seven.BattleState.EnqueueAction(state);

                    #endregion Second


                    Selector.DisableActionHook(true);
                }
                else
                {
                    Seven.BattleState.Commanding.MP -= _spells[_yopt, _xopt].Spell.Cost;

                    state = (AbilityState)Seven.BattleState.Commanding.Ability.Clone();
                    spell = _spells[_xopt, _yopt];

                    state.HitP = spell.Spell.Matp;
                    state.Type = AttackType.Magical;
                    state.MPTurboFactor = spell.MPTurboFactor;
                    state.Elements = spell.Spell.Element;
                    state.Performer = Seven.BattleState.Commanding;
                    state.Action += delegate() { spell.Spell.Action(); };
                }
            }
            else
            {
                _first[0] = _xopt;
                _first[1] = _yopt;
            }
        }



        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);


            int j = Math.Min(_visibleRows + _topRow, _totalRows);

            for (int b = _topRow; b < j; b++)
                for (int a = 0; a < COLUMNS; a++)
                    Text.ShadowedText(g, String.IsNullOrEmpty(_spells[b, a].ID) ? "" : _spells[b, a].Spell.Name,
                        X + x1 + a * xs,
                        Y + (b - _topRow + 1) * ys);


            if (IsControl)
                Shapes.RenderCursor(g,
                    X + x1 + cx + _xopt * xs,
                    Y + cy + (_yopt - _topRow + 1) * ys);

            MagicMenuInfo.Instance.Draw(d);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void Reset()
        {
            _topRow = 0;
            Visible = false;
            MagicMenuInfo.Instance.Visible = false;
            _first = new int[] { -1, -1 };
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
            MagicMenuInfo.Instance.Visible = true;
        }

        public bool IsValid { get { return _totalRows > 0; } }

        public override string Info
        { get { return String.IsNullOrEmpty(_spells[_yopt, _xopt].ID) ? "" : _spells[_yopt, _xopt].Spell.Desc; } }
        public new MagicSpell Selected { get { return _spells[_yopt, _xopt]; } }
    }






}
