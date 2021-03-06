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

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Magic
{       
    internal class Main : ControlMenu, ISelectorUser
    {
        const int COLUMNS = 3;

        #region Layout

        const int x1 = 50;
        const int xs = 200;
        const int ys = 35;
        const int cx = -15;
        const int cy = -8;

        #endregion Layout

        protected int _xopt;
        protected int _yopt;
        private int _topRow;
        private int _totalRows;
        private readonly int _visibleRows = 3;
        protected MagicMenuEntry[,] _spells;

        public Main(Ally ally, IEnumerable<MagicMenuEntry> spells, Menu.ScreenState screenState)
            : base(
                5,
                screenState.Height * 7 / 10 + 20,
                screenState.Width * 3 / 4,
                (screenState.Height * 5 / 20) - 25)
        {
            AssociatedAlly = ally;

            int magicSpellCount = AssociatedAlly.CurrentBattle.Seven.Data.MagicSpellCount;

            _totalRows = (magicSpellCount / COLUMNS) + ((magicSpellCount % COLUMNS == 0) ? 0 : 1);

            _spells = new MagicMenuEntry[_totalRows, COLUMNS];

            foreach (MagicMenuEntry s in spells)
            {
                if (s.Spell.Name == "Toad") HasToad = true;

                _spells[s.Spell.Order / COLUMNS, s.Spell.Order % COLUMNS] = s;
            }

            Squish();

            Reset();
        }

        private void Squish()
        {
            int a, b, r;

            List<int> rowsToUse = new List<int>();

            for (b = 0; b < _totalRows; b++)
            {
                for (a = 0; a < COLUMNS; a++)
                {
                    if (_spells[b, a] != null)
                    {
                        rowsToUse.Add(b);
                        break;
                    }
                }
            }


            MagicMenuEntry[,] newSpells = new MagicMenuEntry[rowsToUse.Count, COLUMNS];

            r = 0;
            foreach (int i in rowsToUse)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    newSpells[r, j] = _spells[i, j];
                }
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

                    if (_yopt > 0)
                    {
                        _yopt--;
                    }
                    if (_topRow > _yopt)
                    {
                        _topRow--;
                    }
                    break;

                case Key.Down:

                    if (_yopt < _totalRows - 1)
                    {
                        _yopt++;
                    }
                    if (_topRow < _yopt - _visibleRows + 1)
                    {
                        _topRow++;
                    }
                    break;

                case Key.Left:

                    if (_xopt > 0)
                    {
                        _xopt--;
                    }
                    break;

                case Key.Right:

                    if (_xopt < COLUMNS - 1)
                    {
                        _xopt++;
                    }
                    break;

                case Key.X:

                    Visible = false;
                    AssociatedAlly.CurrentBattle.Screen.PopControl();
                    Reset();
                    break;

                case Key.Circle:

                    if (Selected != null)
                    {
                        Spell spell = Selected.Spell;
                        
                        if (AssociatedAlly.Frog && spell.Name != "Toad")
                        {                        
                            AssociatedAlly.CurrentBattle.Seven.ShowMessage(c => "!", 500);
                        }
                        else if (CommandingAvailableMP < spell.MPCost)
                        {
                            AssociatedAlly.CurrentBattle.Seven.ShowMessage(c => "!", 500);
                        }
                        else
                        {
                            AssociatedAlly.CurrentBattle.Screen.ActivateSelector(spell.Target, spell.TargetEnemiesFirst);
                        }
                    }
                    break;
            }
        }

        public virtual bool ActOnSelection(IEnumerable<Combatant> targets)
        {
            UseSpell(_xopt, _yopt, targets);

            return true;
        }

        protected void UseSpell(int xopt, int yopt, IEnumerable<Combatant> targets, bool releaseAlly = true)
        {
            Ability spell = _spells[yopt, xopt].Spell;

            spell.Use(AssociatedAlly.CurrentBattle.Commanding, targets, new AbilityModifiers { ResetTurnTimer = releaseAlly});
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Color c;

            int j = Math.Min(_visibleRows + _topRow, _totalRows);


            for (int b = _topRow; b < j; b++)
            {
                for (int a = 0; a < COLUMNS; a++)
                {
                    MagicMenuEntry spell = _spells[b, a];

                    if (spell != null)
                    {
                        if (AssociatedAlly.Frog && spell.Name != "Toad")
                        {
                            c = Colors.GRAY_4;
                        }
                        else
                        {
                            c = Colors.WHITE;
                        }

                        Text.ShadowedText(g, c, spell.Spell.Name,
                            X + x1 + a * xs,
                            Y + (b - _topRow + 1) * ys);
                    }
                }
            }


            if (IsControl)
            {
                Shapes.RenderCursor(g,
                    X + x1 + cx + _xopt * xs,
                    Y + cy + (_yopt - _topRow + 1) * ys);
            }


            AssociatedAlly.CurrentBattle.Screen.MagicInfo.Draw(d, g, width, height, screenChanged);
        }

        public override void Reset()
        {
            //_topRow = 0;
            //_yopt = 0;
            //_xopt = 0;
            Visible = false;
            AssociatedAlly.CurrentBattle.Screen.MagicInfo.Visible = false;
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
            AssociatedAlly.CurrentBattle.Screen.MagicInfo.Visible = true;
        }

        public bool IsValid { get { return _totalRows > 0; } }
        
        public override string Info
        {
            get
            {
                return Selected == null ? String.Empty : Selected.Spell.Desc; 
            }
        }

        public MagicMenuEntry Selected { get { return _spells[_yopt, _xopt]; } }

        public bool HasToad { get; private set; }

        protected virtual int CommandingAvailableMP { get { return AssociatedAlly.CurrentBattle.Commanding.MP; } }
        
        protected Ally AssociatedAlly { get; private set; }
    }
}
