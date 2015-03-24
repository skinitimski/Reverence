

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
