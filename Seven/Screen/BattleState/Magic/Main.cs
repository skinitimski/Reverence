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
        private int _totalRows = (MagicSpell.TOTAL_SPELLS / COLUMNS) + ((MagicSpell.TOTAL_SPELLS % COLUMNS == 0) ? 0 : 1);
        private readonly int _visibleRows = 3;
        protected MagicSpell[,] _spells;

        public Main(IEnumerable<MagicSpell> spells)
            : base(
                5,
                Config.Instance.WindowHeight * 7 / 10 + 20,
                Config.Instance.WindowWidth * 3 / 4,
                (Config.Instance.WindowHeight * 5 / 20) - 25)
        {
            _spells = new MagicSpell[_totalRows, COLUMNS];

            foreach (MagicSpell s in spells)
            {
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


            MagicSpell[,] newSpells = new MagicSpell[rowsToUse.Count, COLUMNS];

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
                    Seven.BattleState.Screen.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    if (_spells[_yopt, _xopt] != null)
                    {
                        Spell spell = _spells[_yopt, _xopt].Spell;
                        
                        if (CommandingAvailableMP >= spell.MPCost)
                        {
                            Seven.BattleState.Screen.GetSelection(spell.Target, spell.TargetEnemiesFirst);
                        }
                    }
                    break;
            }
        }

        public void ActOnSelection()
        {
            UseSpell(_xopt, _yopt);
        }

        protected void UseSpell(int xopt, int yopt, bool releaseAlly = true)
        {
            Spell spell = _spells[yopt, xopt].Spell;
            
            switch (spell.Target)
            {
                case BattleTarget.Combatant:
                case BattleTarget.Ally:
                case BattleTarget.Enemy:
                    //BattleUsage.Use.Call(Seven.BattleState.Screen.TargetSelector.Selected);
                    break;
                    
                case BattleTarget.Group:
                case BattleTarget.Allies:
                case BattleTarget.Enemies:
                    //BattleUsage.Use.Call(Seven.BattleState.Screen.GroupSelector.Selected);
                    break;
                    
                case BattleTarget.Area:
                    //BattleUsage.Use.Call(Seven.BattleState.Screen.AreaSelector.Selected);
                    break;
            }


            //Seven.BattleState.Commanding.UseSpell(

                
            //                    state.HitP = spell.Spell.Matp;
            //                    state.Type = AttackType.Magical;
            //                    state.MPTurboFactor = spell.MPTurboFactor;
            //                    state.Elements = spell.Spell.Element;
            //                    state.Performer = Seven.BattleState.Commanding;
            //                    state.Action += delegate() { spell.Spell.Action(); };
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);


            int j = Math.Min(_visibleRows + _topRow, _totalRows);


            for (int b = _topRow; b < j; b++)
            {
                for (int a = 0; a < COLUMNS; a++)
                {
                    Text.ShadowedText(g, _spells[b, a] != null ? "" : _spells[b, a].Spell.Name,
                        X + x1 + a * xs,
                        Y + (b - _topRow + 1) * ys);
                }
            }


            if (IsControl)
            {
                Shapes.RenderCursor(g,
                    X + x1 + cx + _xopt * xs,
                    Y + cy + (_yopt - _topRow + 1) * ys);
            }

            Seven.BattleState.Screen.MagicInfo.Draw(d);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void Reset()
        {
            _topRow = 0;
            Visible = false;
            Seven.BattleState.Screen.MagicInfo.Visible = false;
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
            Seven.BattleState.Screen.MagicInfo.Visible = true;
        }

        public bool IsValid { get { return _totalRows > 0; } }

        public override string Info
        { get { return _spells[_yopt, _xopt] == null ? "" : _spells[_yopt, _xopt].Spell.Desc; } }

        public MagicSpell Selected { get { return _spells[_yopt, _xopt]; } }

        protected virtual int CommandingAvailableMP { get { return Seven.BattleState.Commanding.MP; } }
    }






}
