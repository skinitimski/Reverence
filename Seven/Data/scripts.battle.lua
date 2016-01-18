

math.randomseed(os.time())


function chooseRandomAlly()
    
    local i = math.random(3) - 1 --Seven.BattleState.Random:Next(3)
    local limit = 0
    
    while BattleState.Allies[i] == nil or BattleState.Allies[i].IsDead do
        i = (i + 1) % Party.PARTY_SIZE
        limit = limit + 1
        if limit > Party.PARTY_SIZE then
           error("Cannot find an ally to target.")
        end
    end
    
    return BattleState.Allies[i]
    
end

function chooseRandomAllyWithHighestHP()
    
    local i = math.random(3) - 1 --Seven.BattleState.Random:Next(3)
    local limit = 0
    
    local a = BattleState.Allies[i]
    
    while true do
    
        local candidate = BattleState.Allies[i]
        
        if candidate ~= nil and not candidate.IsDead then
            
            if a == nil or candidate.HP > a.HP then
                a = candidate
            end
        end
            
        i = (i + 1) % Party.PARTY_SIZE
        limit = limit + 1
        if limit > Party.PARTY_SIZE then
            break
        end
    end
    
    if a == nil then
        error("Couldn't find the ally with the greatest HP.")
    end
    
    return BattleState.Allies[i]
    
end



function chooseRandomEnemy()
    
    local n = BattleState.EnemyList.Count
    local i = math.random(n) - 1 --Seven.BattleState.Random:Next(3)
    
    return BattleState.EnemyList[i]
    
end

function chooseRandomAllyWithoutDarkness()

    local statusFunc = function(a) return a.Darkness end
    local a = chooseRandomAllyWithoutStatus(statusFunc)
    
    return a
    
end

function chooseRandomAllyWithoutStatus(statusFunc)

    local i = math.random(3) - 1 --Seven.BattleState.Random:Next(3)
    local limit = 0
    
    while true do
    
        local a = BattleState.Allies[i]
        
        if a ~= nil and not a.IsDead and not statusFunc(a) then
            do return a end
        end
        
        i = (i + 1) % Party.PARTY_SIZE
        limit = limit + 1
        if limit > Party.PARTY_SIZE then
            break
        end
    end
    
    return nil
    
end

-- roll(min,max,tmin,tmax) : returns true if a random value between min and max (inclusive) is between tmin and tmax (inclusive)
function roll(min, max, tmin, tmax)
    
    real = math.random(min, max)
    
    io.write("\nRolled dice! [",min,",",max,"] ->",real," in [",tmin,",",tmax,"] \n\n")
    
    return tmin <= real and real <= tmax
    
end

-- roll(min,max,test) : returns true if test is equal to a random value between min and max (inclusive)
function roll(min, max, test)
    
    real = math.random(min, max)
    
    io.write("\nRolled dice! [",min,",",max,"] -> ",real," == ",test," \n\n")
    
    return real == test
    
end

-- roll: returns a value between min and max inclusive
function roll(min, max)

    return math.random(min, max)
    
end

    