> test info



test suite: `API`

test name: `LeaderBoard Load Test`

session id: `2025-06-15_05.21.61_session_82c81368`

> scenario stats



scenario: `update_score_scenario`

  - ok count: `3000`

  - fail count: `0`

  - all data: `0.2` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `100`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `3000`, ok = `3000`, RPS = `100`|
|latency (ms)|min = `0.22`, mean = `0.75`, max = `24.42`, StdDev = `0.86`|
|latency percentile (ms)|p50 = `0.65`, p75 = `0.75`, p95 = `1.1`, p99 = `2.22`|
|data transfer (KB)|min = `0.069`, mean = `0.071`, max = `0.073`, all = `0.2` MB|
|||
|name|`update_score`|
|request count|all = `3000`, ok = `3000`, RPS = `100`|
|latency (ms)|min = `0.21`, mean = `0.74`, max = `24.4`, StdDev = `0.86`|
|latency percentile (ms)|p50 = `0.64`, p75 = `0.74`, p95 = `1.09`, p99 = `2.19`|
|data transfer (KB)|min = `0.069`, mean = `0.071`, max = `0.073`, all = `0.2` MB|


> status codes for scenario: `update_score_scenario`



|status code|count|message|
|---|---|---|
|OK|3000||


> scenario stats



scenario: `get_leaderboard_scenario`

  - ok count: `6000`

  - fail count: `0`

  - all data: `11.8` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `200`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `6000`, ok = `6000`, RPS = `200`|
|latency (ms)|min = `0.26`, mean = `0.73`, max = `24.68`, StdDev = `0.88`|
|latency percentile (ms)|p50 = `0.64`, p75 = `0.75`, p95 = `1.08`, p99 = `1.83`|
|data transfer (KB)|min = `0.14`, mean = `2.005`, max = `3.975`, all = `11.8` MB|
|||
|name|`get_leaderboard`|
|request count|all = `6000`, ok = `6000`, RPS = `200`|
|latency (ms)|min = `0.24`, mean = `0.72`, max = `24.67`, StdDev = `0.87`|
|latency percentile (ms)|p50 = `0.64`, p75 = `0.74`, p95 = `1.06`, p99 = `1.81`|
|data transfer (KB)|min = `0.14`, mean = `2.005`, max = `3.975`, all = `11.8` MB|


> status codes for scenario: `get_leaderboard_scenario`



|status code|count|message|
|---|---|---|
|OK|6000||


> scenario stats



scenario: `get_customer_rank_scenario`

  - ok count: `2291`

  - fail count: `709`

  - all data: `0.6` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `100`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `3000`, ok = `2291`, RPS = `76.4`|
|latency (ms)|min = `0.25`, mean = `0.7`, max = `24.44`, StdDev = `0.62`|
|latency percentile (ms)|p50 = `0.65`, p75 = `0.75`, p95 = `1.09`, p99 = `1.52`|
|data transfer (KB)|min = `0.07`, mean = `0.264`, max = `1.088`, all = `0.6` MB|
|||
|name|`get_customer_rank`|
|request count|all = `3000`, ok = `2291`, RPS = `76.4`|
|latency (ms)|min = `0.25`, mean = `0.7`, max = `24.42`, StdDev = `0.62`|
|latency percentile (ms)|p50 = `0.64`, p75 = `0.74`, p95 = `1.08`, p99 = `1.51`|
|data transfer (KB)|min = `0.07`, mean = `0.231`, max = `0.391`, all = `0.5` MB|


|step|failures stats|
|---|---|
|name|`global information`|
|request count|all = `3000`, fail = `709`, RPS = `23.6`|
|latency (ms)|min = `0.31`, mean = `0.84`, max = `15.87`, StdDev = `1.02`|
|latency percentile (ms)|p50 = `0.71`, p75 = `0.84`, p95 = `1.21`, p99 = `3.54`|
|||
|name|`get_customer_rank`|
|request count|all = `3000`, fail = `709`, RPS = `23.6`|
|latency (ms)|min = `0.28`, mean = `0.76`, max = `15.8`, StdDev = `1.02`|
|latency percentile (ms)|p50 = `0.64`, p75 = `0.75`, p95 = `1.09`, p99 = `3.47`|
|data transfer (KB)|min = `0.104`, mean = `0.104`, max = `0.105`, all = `0.1` MB|


> status codes for scenario: `get_customer_rank_scenario`



|status code|count|message|
|---|---|---|
|OK|2291||
|NotFound|709||


