

-- canPartyHeal(p) : returns true only if the party has at least one living character that isn't fully healed (both hp and mp)
function canPartyHeal(p)
    return not ((p[0] == nil or p[0].Death or (p[0].HP == p[0].MaxHP and p[0].MP == p[0].MaxMP))
           and (p[1] == nil or p[1].Death or (p[1].HP == p[1].MaxHP and p[1].MP == p[1].MaxMP))
           and (p[2] == nil or p[2].Death or (p[2].HP == p[2].MaxHP and p[2].MP == p[2].MaxMP)))
end