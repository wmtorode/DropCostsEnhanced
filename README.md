# DropCostsEnhanced
A Mod for HBS's BattleTech PC Game that adds costs for dropping mechs and ammo consumption.

This was inspired by the original drop cost mod: `DropCostPerMech`

## Dependancies

- Custom Components : https://github.com/BattletechModders/CustomComponents
- IRBTModUtils: https://github.com/BattletechModders/IRBTModUtils

## Conflicts

- DropCostPerMech: https://github.com/Morphyum/DropCostPerMech


## Settings

```json

{
  "enableDropCosts" : true,
  "enableAmmoCosts" : true,
  "costFactor" : 0.002,
  "useCostByTons" : false,
  "dropCostPerTon" : 500,
  "roundToNearist" : 10000,
  "debug" : false,
  "trace" : false
}
```

`enableDropCosts` : enable drop costs based on mech value (and optionally mech tonnage)

`enableAmmoCosts` : enable costs based on ammo a mech has expended over the course of a battle

`costFactor` : a float, a multiplier for the drop cost of a mech. 1.0 would be a 100% of the mechs value. the default is 0.2%

`useCostByTons` : enable additional drop costs based on a mechs tonnage.

`dropCostPerTon` : the cost in cbills per ton for dropping a mech

`roundToNearist` : used for rounding a mechs value to the nearest specified denomination

`debug` : used to enable debug logging

`trace` : used to enable trace logging


## Drop Costs

drop costs are based on the value of a mech. the value is determined by taking the cost of the chassis,
and adding the cost of every component on the mech and cost of its armor. this is then multiplied
by `costFactor` to determine the drop price of the mech.

if `useCostByTons` is `true` then the mechs tonnage * `dropCostPerTon` is added to this cost.

## Ammo Costs

When enable, Ammo costs for a mech is calculated for by taking the amount of munitions consumed from all 
ammo bins and weapons with internal ammo and adding a cost for each one.

by default the cost of a munition is (Ammobox or weapon cost)/(Ammo Capacity).

for example a bin of Sniper Ammo costs 40000 and has 5 shots. 40000/5 = 8000. therefore a each shot of a sniper costs 8000 cbills.

### Overriding ammo cost

to change the default cost of a munition, an `AmmoCost` section can be added to the `Custom`
section of an ammo bin or weapon def (providing the weapon has internal ammo) and can set the price of the munition.

example:
```json
{
  "Custom": {
    "AmmoCost" : {
      "PerUnitCost" : 325
    }
  }
}
```

if this custom block was added the the sniper example above than the cost per shot of the sniper would drop from 8000 cbills to 325