# DropCostsEnhanced
A Mod for HBS's BattleTech PC Game that adds costs for dropping mechs and ammo consumption.

This was inspired by the original drop cost mod: `DropCostPerMech` and `GlobalDifficultyByCompany`

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
  "enableHeatCosts" : false,
  "costFactor" : 0.002,
  "useCostByTons" : false,
  "dropCostPerTon" : 500,
  "roundToNearist" : 10000,
  "debug" : false,
  "trace" : false,
  "heatSunkStat" : "CACOverrallHeatSinked",
  "diffMode" : "NotActive",
  "valuePerHalfSkull" : 16500000,
  "defaultMechsToCount" : 8,
  "maxDifficulty" : 25,
  "useDifficultyCostScaling" : false,
  "defaultDifficultyCostModifier" : 1.0,
  "difficultyCostModifiers" : [],
  "useDiffRungs" : false,
  "diffWidgetRungs" : [],
  "additionalUnitCount" : 4,
  "averagedDrops" : 8,
  "excludedContractTypes" : [],
  "excludeFlashpointsFromDropAverage" : true,
  "excludedContractIds" : []
}
```

`enableDropCosts` : enable drop costs based on mech value (and optionally mech tonnage)

`enableAmmoCosts` : enable costs based on ammo a mech has expended over the course of a battle

`enableHeatCosts` : enable costs based on how much heat a mech has sunk over the course of a battle

`costFactor` : a float, a multiplier for the drop cost of a mech. 1.0 would be a 100% of the mechs value. the default is 0.2%

`useCostByTons` : enable additional drop costs based on a mechs tonnage.

`dropCostPerTon` : the cost in cbills per ton for dropping a mech

`roundToNearist` : used for rounding a mechs value to the nearest specified denomination

`debug` : used to enable debug logging

`trace` : used to enable trace logging

`heatSunkStat` : the name of the stat used by CAC to track heat sunk

`useDifficultyCostScaling` : when enabled drop costs can be scaled based on drop difficulty (mech value based)

`defaultDifficultyCostModifier` : when drop difficulty scaling is enabled this is the default modifier for the drop difficulty if one is not defined

`difficultyCostModifiers` : a list of cost scaling modifier objects

`diffMode` : Controls what mode contract/system difficulty operates on. Options:

    - NotActive: The default mode, Global difficulty patches are not applied at all
    - System: Difficulty is based on systemdef defined. This is functionaly the same as NotActive, but Global difficulty patches are applied
    - Company: Difficulty is based on the value of the your deployed forces over the past X missions. Note: When this mode is active, the Difficulty Filter in the starmap will not work (biome filter still works)
    - Reputation: Difficulty is based on your standing with factions. 
    - LegacyCompany: Difficulty is based on the value of your best fieldable units. Note: When this mode is active, the Difficulty Filter in the starmap will not work (biome filter still works)

*The below settings are only active when `diffMode` is set to something other than `NotActive`*

`valuePerHalfSkull` : The value of mech value that decides the lance rating indicator & company difficulty rating when active

`defaultMechsToCount` : The default number of mechs to count for company difficulty when `bigger drops` is not present

`maxDifficulty` : The max contract difficulty that can be generated when not in system mode.

`useDiffRungs` : Allow Lance Selection/skull difficulty widgets display difficulties over 5 skulls, this is represented by the indicators changing colour. colours for each rung of 5 skulls is defined in `diffWidgetRungs`

`diffWidgetRungs`: indicator colour selection, a list of `DifficultyWidgetLevel` objects

`additionalUnitCount`: When using `LegacyCompany` mode count this number of mechs on top of biggerDrops upgrades. default is `4` for legacy versions of BD. if using newer BD, set to `0`.

`averagedDrops` : The number of drops to use for creating averaged drop difficulty when using `Company` difficulty mode

`excludedContractTypes` : Contract Types to exclude from being counted towards averaged drop difficulty

`excludeFlashpointsFromDropAverage` : When true, flashpoint contracts are excluded from affecting averaged drop difficulty

`excludedContractIds` : Contract IDs to exclude from being counted towards averaged drop difficulty

### DifficultyWidgetLevel Objects

```json
{
    "rung" : 1,
    "colour" : "#e6210b",
    "iconOverride": "edited",
    "iconBackingOverride": "edited"
}
```

`rung` : the rung level that this colour should be applied to, each rung level represents a set of 5 skulls, starting with rung level 0, which represents 0.5 - 5 skulls, rung level 1 5.5 - 10 skulls, etc

`colour` : an html colour code, the colour to be applied for this rung level

`iconOverride` : string name of SVG asset which will override the vanilla atlas skull for difficulty indicators

`iconBackingOverride` : string name of SVG asset which will override the vanilla atlas skull which is the shaded grey background for the "unfilled" difficulty indicators. vanilla behavior would be set to the same as `iconOverride` (same shape), but you can define a different background if you want.

## Drop Costs

drop costs are based on the value of a mech. the value is determined by taking the cost of the chassis,
and adding the cost of every component on the mech and cost of its armor. this is then multiplied
by `costFactor` to determine the drop price of the mech.

if `useCostByTons` is `true` then the mechs tonnage * `dropCostPerTon` is added to this cost.

### Adjusting Drop Costs

Drop costs can be adjusted on a per chassis basis by adding a `DropCostFactor` to the custom block of a chassisdef.
this is a multiplier for the overall cost of the mech, values above 1.0 will make the mech more expensive, values below will make it cheaper.

example json:
```json
{
  "Custom": {
    "DropCostFactor" : {
      "DropModifier" : 0.8
    }
  }
}
```

### Drop Cost Difficulty Scaling

Drop costs can be scaled based on the lance's drop difficulty rating, each modifier specifies a range of difficulty values
(inclusive) where that modifier will be applied.

example json:
```json
{
  "minDiff": 0,
  "maxDiff" : 0,
  "modifier" : 1.0
}
```

`minDiff` : the bottom of the difficulty range where this modifier kicks in

`maxDiff` : the top of the range where this modifier is active

`modifier` : the modifier to apply, less than 1 is a discount, greater than 1 is extra cost


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

## Heat Costs

When enabled, Heat costs for a mech are a measure of upkeep and coolant needed to keep a mech's heat dissipation
running at peak conditions. any component (weapons, heatsink, equipment) may carry an upkeep cost by adding a 
`HeatSinkingCost` section to the `Custom`of a component def. at the end of battle all such components will have the upkeep costs
summed in `HeatUpkeepCost`, then multiplied by the amount of heat the unit sunk in battle,
finally, all  `HeatUpkeepMult` will multiply on this base cost the result is its upkeep fee.

example custom:
```json
{
  "Custom": {
    "HeatSinkingCost" : {
      "HeatUpkeepCost" : 25,
      "HeatUpkeepMult" : 1.1
    }
  }
}
```

### Example Heat
We have a black Knight, with DHS kit. this kit has an upkeep cost of 25 cbills. it also has a coolant unit with an upkeep cost of 5 cbills and has 2 heat exchanger
will an upkeep Multiplier of 1.2 each.

In battle this unit sinks 500 heat.

```
heat Cost = (((25 + 5) * 500) * 1.2) * 1.2
heat Cost = ((30 * 500) * 1.2) * 1.2
heat Cost = (15000 * 1.2) * 1.2
heat Cost = 18000 * 1.2
heat Cost = 21600
```
          
