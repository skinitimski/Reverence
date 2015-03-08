using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.BattleSimulator
{

    public class EnemySkillMenu : ControlMenu, ISelectorUser
    {
        private const int COLUMNS = 2;

        #region Layout

        const int x1 = 50;
        const int xs = 250;
        const int ys = 35;
        const int cx = -15;
        const int cy = -8;

        #endregion Layout

        private int _xopt;
        private int _yopt;
        private int _topRow;
        private int _totalRows = (EnemySkillMateria.TOTAL_ENEMY_SKILLS / COLUMNS) + ((EnemySkillMateria.TOTAL_ENEMY_SKILLS % COLUMNS == 0) ? 0 : 1);
        private readonly int _visibleRows = 3;

        /// <summary>Spells: row, column.</summary>
        private Spell[,] _spells;

        public EnemySkillMenu(EnemySkillMateria esm)
            : base(
                5,
                Globals.HEIGHT * 7 / 10 + 20,
                Globals.WIDTH * 3 / 4,
                (Globals.HEIGHT * 5 / 20) - 25)
        {
            _spells = new Spell[_totalRows, COLUMNS];

            for (int i = 0; i < EnemySkillMateria.TOTAL_ENEMY_SKILLS; i++)
                if (((esm.AP >> i) & 1) > 0)
                {
                    Spell s = esm.EnemySkills[i];
                    _spells[s.Order / COLUMNS, s.Order % COLUMNS] = s;
                }

            Reset();
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
                    BattleScreen.Instance.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    if (!String.IsNullOrEmpty(_spells[_yopt, _xopt].ID))
                        if (Game.Battle.Commanding.MP >= _spells[_yopt, _xopt].Cost)
                            _spells[_yopt, _xopt].Dispatch();
                    break;
            }
        }
        public void ActOnSelection()
        {
            Game.Battle.Commanding.MP -= _spells[_yopt, _xopt].Cost;

            AbilityState state;

            state = (AbilityState)Game.Battle.Commanding.Ability.Clone();

            state.Performer = Game.Battle.Commanding;

            state.Action += delegate() { _spells[_xopt, _yopt].Action(); };
        }


        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);


            int j = Math.Min(_visibleRows + _topRow, _totalRows);

            for (int b = _topRow; b < j; b++)
                for (int a = 0; a < COLUMNS; a++)
                    Graphics.ShadowedText(g, String.IsNullOrEmpty(_spells[b, a].ID) ? "" : _spells[b, a].Name,
                        X + x1 + a * xs,
                        Y + (b - _topRow + 1) * ys);


            if (IsControl)
                Graphics.RenderCursor(g,
                    X + x1 + cx + _xopt * xs,
                    Y + cy + (_yopt - _topRow + 1) * ys);

            EnemySkillMenuInfo.Instance.Draw(d);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void Reset()
        {
            _topRow = 0;
            Visible = false;
            EnemySkillMenuInfo.Instance.Visible = false;
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
            EnemySkillMenuInfo.Instance.Visible = true;
        }

        public bool IsValid { get { return _totalRows > 0; } }

        public override string Info
        { get { return String.IsNullOrEmpty(_spells[_yopt, _xopt].ID) ? "" : _spells[_yopt, _xopt].Desc; } }
        public new Spell Selected { get { return _spells[_yopt, _xopt]; } }
    }




    internal sealed class EnemySkillMenuInfo : Menu
    {
        public static EnemySkillMenuInfo Instance;

        #region Layout

        const int x0 = 15; // ability col
        const int x1 = 40; // mpneeded col
        const int x2 = 90; // slash col
        const int x3 = 130; // mptot col
        const int y0 = 27; // 
        const int ys = 27;

        #endregion Layout

        static EnemySkillMenuInfo()
        {
            Instance = new EnemySkillMenuInfo();
        }
        private EnemySkillMenuInfo()
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

            Spell s = Game.Battle.Commanding.EnemySkillMenu.Selected;

            int row = 0;

            if (!String.IsNullOrEmpty(s.ID))
            {
                string cost = s.Cost.ToString();
                Graphics.ShadowedText(g, "MP Req", X + x1, Y + y0);

                row++;

                cost = cost + "/";
                te = g.TextExtents(cost);
                Graphics.ShadowedText(g, cost, X + x2 - te.Width, Y + y0 + (row * ys));

                string tot = Game.Battle.Commanding.MP.ToString();
                te = g.TextExtents(tot);
                Graphics.ShadowedText(g, tot, X + x3 - te.Width, Y + y0 + (row * ys));

                row++;
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }


}
