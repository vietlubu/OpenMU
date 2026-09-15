#!/bin/sh
set -eu

repo_root=$(CDPATH= cd -- "$(dirname -- "$0")/../.." && pwd)
compose_dir="$repo_root/deploy/all-in-one"

# Lấy version từ file deploy chính để không phải update 2 nơi
expected_data_version=$(grep -E '^expected_data_version=[0-9]+' "$compose_dir/deploy-lenovo.sh" | cut -d '=' -f 2)

cd "$compose_dir"

echo "==> Building local image from current source..."
docker compose build openmu-startup

echo "==> Stopping running server..."
docker compose stop openmu-startup

echo "==> Applying mandatory configuration updates to local DB..."
if ! docker compose run --rm --no-deps openmu-startup -applymandatoryupdates; then
    echo "Update failed. Restarting previous container state..." >&2
    docker compose up -d --no-deps openmu-startup
    exit 1
fi

echo "==> Starting openmu-startup..."
docker compose up -d --no-deps --force-recreate openmu-startup

echo "==> Waiting for server to become ready..."
elapsed=0
until docker compose logs openmu-startup 2>&1 | grep -q "Host started"; do
    if [ "$elapsed" -ge 240 ]; then
        echo "OpenMU did not become ready within 240 seconds." >&2
        docker compose logs --tail 100 openmu-startup >&2
        exit 1
    fi
    sleep 5
    elapsed=$((elapsed + 5))
done

echo "==> Verifying database version..."
installed_data_version=$(docker exec database psql -U postgres -d openmu -Atc \
    "select \"CurrentInstalledVersion\" from config.\"ConfigurationUpdateState\" where \"InitializationKey\" = 'season6';")

if [ "${installed_data_version:-0}" -lt "$expected_data_version" ]; then
    echo "Database update failed: installed=$installed_data_version expected=$expected_data_version" >&2
    exit 2
fi

echo "================================================="
echo "✅ Local deployment successful!"
echo "Season 6 data version: $installed_data_version"
echo "Admin Panel: http://localhost:8082"
echo "================================================="
