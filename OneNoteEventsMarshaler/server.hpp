#pragma once

namespace Server {

void lock() noexcept;
void unlock() noexcept;

void notify_object_created() noexcept;
void notify_object_destroyed() noexcept;

}
