# Vortex Analytics Documentation (Unreal Engine)

The Vortex Plugin for Unreal Engine is a GameInstance Subsystem. This means it is a "set-and-forget" manager that is automatically created when the game starts and persists across level loads.

## Installation & Setup
1. Dossier Plugins: Create a Plugins/Vortex folder at the root of your Unreal project.
2. Files: Drop the Source, Resources (if any), and Vortex.uplugin into that folder.
3. Generate Files: Right-click your .uproject file and select Generate Visual Studio project files.
4. Build: Open your solution and build your project.

### Project Settings

Vortex integrates directly into the Unreal UI. Go to Edit > Project Settings > Plugins > Vortex Analytics. You can toggle analytics globally or specifically for the Editor or Shipping builds to prevent dev data from polluting your production metrics.

# Initialization

Before tracking events, you must initialize the manager with your credentials. This is typically done in your `BeginPlay` of the GameMode or a Login script.

## Blueprints (Recommended)
Search for the Get Analytics Manager node. It is available globally.

## C++

```cpp
#include "AnalyticsManager.h"

UAnalyticsManager* Analytics = UAnalyticsManager::Get(GetWorld());
if (Analytics)
{
    Analytics->Init(TEXT("my_tenant_id"), TEXT("https://in.vortexanalytics.io"), TEXT("PS5"));
}
```

## Tracking Events

### Simple Events
Track a basic action without additional data.
- Blueprint: Use the Track Event node.
- C++: Analytics->TrackEvent(TEXT("level_started"));

### Events with Payloads

If you want to send a string (like a JSON string or a simple tag):
- Blueprint: Provide the string in the Props input of the Track Event node.
- C++: Analytics->TrackEvent(TEXT("item_bought"), TEXT("shield_01"));

### Events with Structured Data (Maps)

To send multiple key-value pairs, use a Map.
- Blueprint: Use the Track Event With Props node. Connect it to a Make Map node.
- C++:

```cpp
TMap<FString, FString> Params;
Params.Add(TEXT("health"), TEXT("50"));
Params.Add(TEXT("zone"), TEXT("FireDungeon"));

Analytics->TrackEventWithProps(TEXT("player_status"), Params);
```

## Batching & Optimization

### Manual Batching
If you have many high-frequency events (e.g., every bullet hit), use batching to save bandwidth.
1. Add to Batch: Use Batched Track Event. This saves the event locally in a queue.
2. Flush: Call Flush Manual Batch to send everything in the queue in one single HTTP request.

### Automatic Batching
You can let the system handle the network calls in the background:

```cpp
// Flushes the internal queue every 20 seconds automatically
Analytics->SetAutoBatching(true, 20.0f);
```

## Custom Data

### Overview
Custom data allows you to attach persistent metadata to all subsequent events. This is useful for tracking user session information, device settings, or any other contextual data that applies to multiple events.

### Setting Custom Data

Use `SetCustomData()` to attach metadata that will be automatically included in every event:

```cpp
// C++:
TMap<FString, FString> CustomData;
CustomData.Add(TEXT("user_tier"), TEXT("premium"));
CustomData.Add(TEXT("region"), TEXT("EU"));
Analytics->SetCustomData(CustomData);
```

In Blueprint, use the **Set Custom Data** node and connect it to a **Make Map** node:

```
Make Map → [key: "user_tier", value: "premium"]
          [key: "region", value: "EU"]
            → Set Custom Data
```

### Clearing Custom Data

To remove custom data from future events:

```cpp
// C++:
Analytics->ClearCustomData();
```

### Behavior Details

- **When empty**: The `custom` field is **completely omitted** from the JSON payload. This keeps the request size minimal.
- **When set**: The custom data is automatically serialized to JSON and included in every event until cleared.
- **Automatic inclusion**: Custom data is automatically sent with all tracking methods (single events, batched events, etc.).
- **No validation needed**: The Map serialization handles the JSON conversion for you.