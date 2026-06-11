"use client"

import { useState, useEffect } from "react"
import { Separator } from "@/components/ui/separator"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import { toast } from "sonner"
import { adminApi } from "@/lib/api"

interface UserProfile {
  email: string
  firstName: string
  lastName: string
  role: string
  userId: string
}

export default function ProfilePage() {
  const [profile, setProfile] = useState<UserProfile | null>(null)
  const [editing, setEditing] = useState(false)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [showSuccess, setShowSuccess] = useState(false)
  const [formData, setFormData] = useState({ firstName: "", lastName: "" })

  useEffect(() => {
    const token = localStorage.getItem("nexus_token")
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
        console.log("Profile loaded:", newProfile)
        console.log("Role check:", newProfile.role === "SuperAdmin")
        setProfile(newProfile)
        setFormData({
          firstName: newProfile.firstName,
          lastName: newProfile.lastName,
        })
      } catch (error) {
        console.error("Failed to decode token:", error)
      }
    }
    setLoading(false)
  }, [])

  const handleSave = async () => {
    setSaving(true)
    try {
      // Call API to update profile
      const response = await adminApi.updateProfile(formData.firstName, formData.lastName)

      // Store the new JWT token with updated user info
      localStorage.setItem("nexus_token", response.data.accessToken)

      // Reload profile data from the new token
      const decoded = JSON.parse(atob(response.data.accessToken.split(".")[1]))
      const updated = {
        email: decoded.email,
        firstName: decoded.given_name || "User",
        lastName: decoded.family_name || "",
        role: decoded.role,
        userId: decoded.sub,
      }
      setProfile(updated)

      // Update localStorage so TopBar reflects the change
      localStorage.setItem("user_info", JSON.stringify({
        firstName: updated.firstName,
        lastName: updated.lastName,
        email: updated.email
      }))

      // Dispatch custom event to notify TopBar of user info change
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
    return <div className="text-muted-foreground">Loading...</div>
  }

  if (!profile) {
    return <div className="text-destructive">Failed to load profile</div>
  }

  const fullName = `${profile.firstName} ${profile.lastName}`.trim()
  const initials = `${profile.firstName[0] || ""}${profile.lastName[0] || ""}`.toUpperCase()

  return (
    <div className="max-w-2xl space-y-6">
      {showSuccess && (
        <div className="bg-green-900/20 border border-green-500 rounded-lg p-4 text-green-300 flex items-center gap-3 animate-in fade-in">
          <span>✅</span>
          <span className="font-medium">Profile updated successfully!</span>
        </div>
      )}

      {/* Profile Header */}
      <div className="bg-card border border-border rounded-xl p-6 space-y-4">
        <h1 className="text-2xl font-bold text-foreground">Profile</h1>
        <Separator />

        <div className="flex items-center gap-4">
          <Avatar className="w-16 h-16">
            <AvatarFallback className="bg-[--color-brand-blue] text-white text-lg font-bold">
              {initials || "A"}
            </AvatarFallback>
          </Avatar>
          <div>
            <p className="text-lg font-semibold text-foreground">{fullName}</p>
            <p className="text-sm text-muted-foreground">{profile.email}</p>
          </div>
        </div>
      </div>

      {/* Profile Details */}
      <div className="bg-card border border-border rounded-xl p-6 space-y-5">
        <h2 className="text-base font-semibold text-foreground">Account Information</h2>
        <Separator />

        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-foreground mb-1.5">First Name</label>
            {editing && profile ? (
              <input
                type="text"
                value={formData.firstName}
                onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                className="w-full px-3 py-2 text-sm border-2 border-[--color-brand-blue] rounded-lg bg-white text-black focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
              />
            ) : (
              <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">
                {profile.firstName}
              </p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-foreground mb-1.5">Last Name</label>
            {editing && profile ? (
              <input
                type="text"
                value={formData.lastName}
                onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                className="w-full px-3 py-2 text-sm border-2 border-[--color-brand-blue] rounded-lg bg-white text-black focus:outline-none focus:ring-2 focus:ring-[--color-brand-blue]"
              />
            ) : (
              <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">
                {profile.lastName || "—"}
              </p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-foreground mb-1.5">Email Address</label>
            <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">
              {profile.email}
            </p>
          </div>

          <div>
            <label className="block text-sm font-medium text-foreground mb-1.5">Role</label>
            <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground">
              <span className="inline-block px-2 py-1 rounded bg-[--color-brand-blue] text-white text-xs font-semibold">
                {profile.role}
              </span>
            </p>
          </div>

          <div>
            <label className="block text-sm font-medium text-foreground mb-1.5">User ID</label>
            <p className="px-3 py-2 text-sm border border-border rounded-lg bg-muted text-foreground font-mono text-xs">
              {profile.userId}
            </p>
          </div>
        </div>

        <div className="pt-4 space-y-4">
          {profile && !editing && (
            <button
              onClick={() => setEditing(true)}
              className="px-4 py-2 bg-[--color-brand-blue] hover:bg-blue-700 text-white font-medium rounded-lg transition-colors cursor-pointer"
            >
              Edit Profile
            </button>
          )}

          {profile && editing && (
            <div className="flex gap-3">
              <button
                onClick={handleSave}
                disabled={saving}
                className="px-4 py-2 bg-[--color-brand-blue] hover:bg-blue-700 text-white font-medium rounded-lg transition-colors disabled:opacity-50"
              >
                {saving ? "Saving..." : "Save Changes"}
              </button>
              <button
                onClick={() => {
                  setEditing(false)
                  setFormData({
                    firstName: profile.firstName,
                    lastName: profile.lastName,
                  })
                }}
                className="px-4 py-2 border border-border text-foreground hover:bg-muted font-medium rounded-lg transition-colors"
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
