"use client"

import { useState, useEffect } from "react"
import { toast } from "sonner"
import { adminApi } from "@/lib/api"

interface UserProfile {
  email: string
  firstName: string
  lastName: string
  role: string
  userId: string
}

const readonlyCls = "px-3 py-2 text-sm border border-gray-200 dark:border-gray-800 rounded-lg bg-gray-100 dark:bg-gray-800 text-gray-900 dark:text-white"
const editCls = "w-full px-3 py-2 text-sm border-2 border-brand-500 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-brand-500/30"
const labelCls = "block text-sm font-medium text-gray-900 dark:text-white mb-1.5"

export default function ProfilePage() {
  const [profile, setProfile] = useState<UserProfile | null>(null)
  const [editing, setEditing] = useState(false)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [showSuccess, setShowSuccess] = useState(false)
  const [formData, setFormData] = useState({ firstName: "", lastName: "" })

  useEffect(() => {
    const token = localStorage.getItem("litxus_token")
    if (token) {
      try {
        const decoded = JSON.parse(atob(token.split(".")[1]))
        const newProfile = {
          email: decoded.email,
          firstName: decoded.given_name || "User",
          lastName: decoded.family_name || "",
          role: decoded.role,
          userId: decoded.sub,
        }
        setProfile(newProfile)
        setFormData({ firstName: newProfile.firstName, lastName: newProfile.lastName })
      } catch (error) {
        console.error("Failed to decode token:", error)
      }
    }
    setLoading(false)
  }, [])

  const handleSave = async () => {
    setSaving(true)
    try {
      const response = await adminApi.updateProfile(formData.firstName, formData.lastName)
      localStorage.setItem("litxus_token", response.data.accessToken)
      const decoded = JSON.parse(atob(response.data.accessToken.split(".")[1]))
      const updated = {
        email: decoded.email,
        firstName: decoded.given_name || "User",
        lastName: decoded.family_name || "",
        role: decoded.role,
        userId: decoded.sub,
      }
      setProfile(updated)
      localStorage.setItem("user_info", JSON.stringify({
        firstName: updated.firstName,
        lastName: updated.lastName,
        email: updated.email
      }))
      window.dispatchEvent(new CustomEvent("userInfoUpdated", { detail: updated }))
      setShowSuccess(true)
      setEditing(false)
      setTimeout(() => setShowSuccess(false), 3000)
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Failed to save profile")
    } finally {
      setSaving(false)
    }
  }

  if (loading) {
    return <div className="text-gray-500 dark:text-gray-400">Loading...</div>
  }

  if (!profile) {
    return <div className="text-red-500">Failed to load profile</div>
  }

  const fullName = `${profile.firstName} ${profile.lastName}`.trim()
  const initials = `${profile.firstName[0] || ""}${profile.lastName[0] || ""}`.toUpperCase()

  return (
    <div className="max-w-2xl space-y-6">
      {showSuccess && (
        <div className="bg-green-900/20 border border-green-500 rounded-lg p-4 text-green-300 flex items-center gap-3">
          <span>✅</span>
          <span className="font-medium">Profile updated successfully!</span>
        </div>
      )}

      {/* Profile Header */}
      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6 space-y-4">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Profile</h1>
        <div className="h-px w-full bg-gray-200 dark:bg-gray-800" />
        <div className="flex items-center gap-4">
          <div className="w-16 h-16 rounded-full bg-brand-500 flex items-center justify-center text-white text-xl font-bold shrink-0">
            {initials || "A"}
          </div>
          <div>
            <p className="text-lg font-semibold text-gray-900 dark:text-white">{fullName}</p>
            <p className="text-sm text-gray-500 dark:text-gray-400">{profile.email}</p>
          </div>
        </div>
      </div>

      {/* Profile Details */}
      <div className="bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-800 rounded-xl p-6 space-y-5">
        <h2 className="text-base font-semibold text-gray-900 dark:text-white">Account Information</h2>
        <div className="h-px w-full bg-gray-200 dark:bg-gray-800" />

        <div className="space-y-4">
          <div>
            <label className={labelCls}>First Name</label>
            {editing ? (
              <input
                type="text"
                value={formData.firstName}
                onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                className={editCls}
              />
            ) : (
              <p className={readonlyCls}>{profile.firstName}</p>
            )}
          </div>

          <div>
            <label className={labelCls}>Last Name</label>
            {editing ? (
              <input
                type="text"
                value={formData.lastName}
                onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                className={editCls}
              />
            ) : (
              <p className={readonlyCls}>{profile.lastName || "—"}</p>
            )}
          </div>

          <div>
            <label className={labelCls}>Email Address</label>
            <p className={readonlyCls}>{profile.email}</p>
          </div>

          <div>
            <label className={labelCls}>Role</label>
            <p className={readonlyCls}>
              <span className="inline-block px-2 py-1 rounded bg-brand-500 text-white text-xs font-semibold">
                {profile.role}
              </span>
            </p>
          </div>

          <div>
            <label className={labelCls}>User ID</label>
            <p className={readonlyCls + " font-mono text-xs"}>{profile.userId}</p>
          </div>
        </div>

        <div className="pt-4 space-y-4">
          {!editing && (
            <button
              onClick={() => setEditing(true)}
              className="px-4 py-2 bg-brand-500 hover:bg-brand-600 text-white font-medium rounded-lg transition-colors cursor-pointer"
            >
              Edit Profile
            </button>
          )}

          {editing && (
            <div className="flex gap-3">
              <button
                onClick={handleSave}
                disabled={saving}
                className="px-4 py-2 bg-brand-500 hover:bg-brand-600 text-white font-medium rounded-lg transition-colors disabled:opacity-50"
              >
                {saving ? "Saving..." : "Save Changes"}
              </button>
              <button
                onClick={() => {
                  setEditing(false)
                  setFormData({ firstName: profile.firstName, lastName: profile.lastName })
                }}
                className="px-4 py-2 border border-gray-200 dark:border-gray-800 text-gray-900 dark:text-white hover:bg-gray-100 dark:hover:bg-gray-800 font-medium rounded-lg transition-colors"
              >
                Cancel
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
