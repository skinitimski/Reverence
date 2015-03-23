
--Party = luanet.import_type("Atmosphere.Reverence.Seven.Party")






math.randomseed(os.time())








function chooseRandomAlly()
    
    i = math.random(3) --math.Seven.BattleState.Random:Next(3)
    limit = 0
    
    while BattleState.Allies[i] == nil or BattleState.Allies[i].IsDead do
        i = (i + 1) % Party.PARTY_SIZE
        limit = limit + 1
        if limit > Party.PARTY_SIZE then
           -- i = Party.PARTY_SIZE + 1
        end
    end
    
    return BattleState.Allies[i]
    
end
