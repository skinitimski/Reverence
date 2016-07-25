using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.EnemySkill
{
    internal class Main : ControlMenu, ISelectorUser
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
        private int _totalRows;
        private readonly int _visibleRows = 3;
        
        /// <summary>Spells: row, column.</summary>
        private Spell[,] _spells;
        
        public Main(SevenBattleState battleState, EnemySkillMateria esm, Menu.ScreenState screenState)
            : base(
                5,
                screenState.Height * 7 / 10 + 20,
                screenState.Width * 3 / 4,
                (screenState.Height * 5 / 20) - 25)
        {
            BattleState = battleState;

            int enemySkillCount = battleState.Seven.Data.EnemySkillCount;

            _totalRows = (enemySkillCount / COLUMNS) + ((enemySkillCount % COLUMNS == 0) ? 0 : 1);

            _spells = new Spell[_totalRows, COLUMNS];
            
            for (int i = 0; i < enemySkillCount; i++)
            {
                if (((esm.AP >> i) & 1) > 0)
                {
                    Spell s = battleState.Seven.Data.GetEnemySkill(esm.Abilities.ElementAt(i));

                    _spells[s.Order / COLUMNS, s.Order % COLUMNS] = s;
                }
            }
            
            Reset();
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
                    BattleState.Screen.PopControl();
                    Reset();
                    break;

                case Key.Circle:
                    if (Selected != null)
                    {
                        Spell spell = Selected;
                        
                        if (BattleState.Commanding.MP >= spell.MPCost)
                        {
                            BattleState.Screen.ActivateSelector(spell.Target, spell.TargetEnemiesFirst);
                        }
                        else
                        {
                            BattleState.Seven.ShowMessage(c => "!", 500);
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
            Ability spell = _spells[yopt, xopt];
            
            spell.Use(BattleState.Commanding, targets, new AbilityModifiers { ResetTurnTimer = releaseAlly});
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
                    Spell enemySkill = _spells[b, a];
                    
                    if (enemySkill != null)
                    {
                        Text.ShadowedText(g, enemySkill.Name,
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


            BattleState.Screen.EnemySkillInfo.Draw(d);
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void Reset()
        {
            _topRow = 0;
            Visible = false;
            BattleState.Screen.EnemySkillInfo.Visible = false;
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
            BattleState.Screen.EnemySkillInfo.Visible = true;
        }
        
        public bool IsValid { get { return _totalRows > 0; } }
        
        public override string Info
        {
            get
            {
                return Selected == null ? String.Empty : Selected.Desc; 
            }
        }

        public Spell Selected { get { return _spells[_yopt, _xopt]; } }
        
        private SevenBattleState BattleState { get; set; }
    }
}

